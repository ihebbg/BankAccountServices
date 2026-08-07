output "ecr_repository_url" {
  description = "Push the Docker image to this repository."
  value       = aws_ecr_repository.api.repository_url
}

output "application_url" {
  description = "Public API URL (HTTP for this learning lab)."
  value       = "http://${aws_lb.api.dns_name}"
}

output "ecs_cluster_name" {
  value = aws_ecs_cluster.main.name
}

output "ecs_service_name" {
  value = aws_ecs_service.api.name
}

output "cloudwatch_log_group" {
  value = aws_cloudwatch_log_group.api.name
}
