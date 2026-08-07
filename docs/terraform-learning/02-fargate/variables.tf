variable "aws_region" {
  description = "AWS region used by every resource."
  type        = string
  default     = "eu-west-3"
}

variable "project_name" {
  description = "Short lowercase name used to prefix resources."
  type        = string
  default     = "bankaccount"
}

variable "environment" {
  description = "Deployment environment."
  type        = string
  default     = "lab"
}

variable "image_tag" {
  description = "Immutable Docker image tag already pushed to the ECR repository."
  type        = string
  default     = "latest"
}

variable "desired_count" {
  description = "Number of Fargate tasks. Set to 0 before the first image push."
  type        = number
  default     = 0

  validation {
    condition     = var.desired_count >= 0
    error_message = "desired_count must be zero or greater."
  }
}

variable "container_cpu" {
  description = "Fargate task CPU units (256 = 0.25 vCPU)."
  type        = number
  default     = 256
}

variable "container_memory" {
  description = "Fargate task memory in MiB."
  type        = number
  default     = 512
}

variable "db_connection_secret_arn" {
  description = "ARN of a Secrets Manager secret whose entire value is the MySQL connection string."
  type        = string
  sensitive   = true
}

variable "jwt_key_secret_arn" {
  description = "ARN of a Secrets Manager secret whose entire value is the JWT signing key (32+ bytes)."
  type        = string
  sensitive   = true
}

variable "s3_bucket_name" {
  description = "Existing S3 bucket used by the API."
  type        = string
}

variable "s3_prefix" {
  description = "Optional object-key prefix reserved for this application."
  type        = string
  default     = "bankaccount"
}
