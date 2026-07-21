using System.Globalization;
using Amazon.S3;
using Amazon.S3.Model;
using BankAccountServices.Configuration;
using BankAccountServices.DTOs.S3;
using BankAccountServices.Services.Interfaces;

namespace BankAccountServices.Services
{
	public class S3StorageService(IAmazonS3 s3Client, S3Settings settings) : IS3StorageService
	{
		private const string DefaultContentType = "application/octet-stream";
		private readonly IAmazonS3 _s3Client = s3Client;
		private readonly S3Settings _settings = settings;

		public async Task<S3FileResponseDTO> UploadFileAsync(IFormFile file, string? folder, CancellationToken cancellationToken = default)
		{
			if (file == null || file.Length == 0)
			{
				throw new ArgumentException("Le fichier est obligatoire.", nameof(file));
			}

			var key = BuildObjectKey(file.FileName, folder);
			var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? DefaultContentType : file.ContentType;

			await using var stream = file.OpenReadStream();
			var request = new PutObjectRequest
			{
				BucketName = _settings.BucketName,
				Key = key,
				InputStream = stream,
				ContentType = contentType
			};
			request.Metadata.Add("original-file-name", file.FileName);

			await _s3Client.PutObjectAsync(request, cancellationToken);

			return new S3FileResponseDTO
			{
				Key = key,
				FileName = Path.GetFileName(file.FileName),
				ContentType = contentType,
				Size = file.Length,
				Url = BuildPublicUrl(key)
			};
		}

		public async Task<S3DownloadFileDTO> DownloadFileAsync(string key, CancellationToken cancellationToken = default)
		{
			key = NormalizeExistingKey(key);

			var response = await _s3Client.GetObjectAsync(_settings.BucketName, key, cancellationToken);
			var fileName = Path.GetFileName(key);

			return new S3DownloadFileDTO
			{
				Content = response.ResponseStream,
				ContentType = string.IsNullOrWhiteSpace(response.Headers.ContentType) ? DefaultContentType : response.Headers.ContentType,
				FileName = string.IsNullOrWhiteSpace(fileName) ? "download" : fileName
			};
		}

		public Task<S3PresignedUrlResponseDTO> GetPresignedUrlAsync(string key, int? expirationMinutes = null)
		{
			key = NormalizeExistingKey(key);

			var minutes = expirationMinutes ?? _settings.PreSignedUrlExpirationMinutes;
			if (minutes <= 0)
			{
				throw new ArgumentException("La durée d'expiration doit être supérieure à zéro.", nameof(expirationMinutes));
			}

			var expiresAtUtc = DateTime.UtcNow.AddMinutes(minutes);
			var request = new GetPreSignedUrlRequest
			{
				BucketName = _settings.BucketName,
				Key = key,
				Expires = expiresAtUtc
			};

			return Task.FromResult(new S3PresignedUrlResponseDTO
			{
				Key = key,
				Url = _s3Client.GetPreSignedURL(request),
				ExpiresAtUtc = expiresAtUtc
			});
		}

		public async Task DeleteFileAsync(string key, CancellationToken cancellationToken = default)
		{
			key = NormalizeExistingKey(key);
			await _s3Client.DeleteObjectAsync(_settings.BucketName, key, cancellationToken);
		}

		private string BuildObjectKey(string fileName, string? folder)
		{
			var segments = new List<string>();
			segments.AddRange(SplitPath(_settings.Prefix));
			segments.AddRange(SplitPath(folder));
			segments.Add(DateTime.UtcNow.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
			segments.Add($"{Guid.NewGuid():N}_{SanitizeFileName(fileName)}");

			return string.Join("/", segments);
		}

		private string NormalizeExistingKey(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentException("La clé S3 est obligatoire.", nameof(key));
			}

			var normalizedKey = key.Trim().TrimStart('/').Replace('\\', '/');
			var segments = SplitPath(normalizedKey).ToList();
			if (segments.Count == 0)
			{
				throw new ArgumentException("La clé S3 est invalide.", nameof(key));
			}

			normalizedKey = string.Join("/", segments);
			var prefix = string.Join("/", SplitPath(_settings.Prefix));
			if (!string.IsNullOrWhiteSpace(prefix) &&
				!normalizedKey.Equals(prefix, StringComparison.Ordinal) &&
				!normalizedKey.StartsWith($"{prefix}/", StringComparison.Ordinal))
			{
				throw new InvalidOperationException($"La clé S3 doit commencer par le préfixe configuré '{prefix}/'.");
			}

			return normalizedKey;
		}

		private string? BuildPublicUrl(string key)
		{
			if (string.IsNullOrWhiteSpace(_settings.PublicBaseUrl))
			{
				return null;
			}

			return $"{_settings.PublicBaseUrl}/{Uri.EscapeDataString(key).Replace("%2F", "/", StringComparison.OrdinalIgnoreCase)}";
		}

		private static IEnumerable<string> SplitPath(string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				yield break;
			}

			var segments = value.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			foreach (var segment in segments)
			{
				if (segment is "." or "..")
				{
					continue;
				}

				var cleanSegment = SanitizeSegment(segment);
				if (!string.IsNullOrWhiteSpace(cleanSegment))
				{
					yield return cleanSegment;
				}
			}
		}

		private static string SanitizeFileName(string fileName)
		{
			var safeFileName = SanitizeSegment(Path.GetFileName(fileName));
			return string.IsNullOrWhiteSpace(safeFileName) ? "file" : safeFileName;
		}

		private static string SanitizeSegment(string value)
		{
			var invalidChars = Path.GetInvalidFileNameChars();
			var cleanChars = value.Select(character => invalidChars.Contains(character) ? '-' : character).ToArray();
			return new string(cleanChars).Trim('.');
		}
	}
}
