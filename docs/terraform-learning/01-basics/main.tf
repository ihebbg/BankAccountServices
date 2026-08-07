terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
  }
}

provider "aws" {
  region = "eu-west-3"
}

resource "aws_s3_bucket" "demo_bucket" {
  bucket = "iheb-terraform-demo-2026"

  tags = {
    Name        = "TerraformDemo"
    Environment = "Dev"
  }
}