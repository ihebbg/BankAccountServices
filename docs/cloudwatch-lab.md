# CloudWatch lab for BankAccountServices

Goal: follow one API request from this ASP.NET Core project into CloudWatch Logs, then create a simple metric and alarm.

## 1. What this project now emits

The API writes Serilog files under:

```text
Logs/log*.txt
```

Each HTTP request is logged with method, path, status code, and duration:

```text
HTTP GET /swagger responded 200 in 45 ms
HTTP POST /api/customer responded 500 in 12 ms
```

Unhandled exceptions are also logged with the exception stack trace.

## 2. What CloudWatch should receive

Use CloudWatch for three things first:

- CloudWatch Logs: application logs from `Logs/log*.txt`
- CloudWatch Metrics: EC2 CPU, memory, and disk metrics from the CloudWatch Agent
- CloudWatch Alarms: alert when the API produces errors

## 3. EC2/IIS setup

Attach an IAM role to the EC2 instance with the AWS managed policy:

```text
CloudWatchAgentServerPolicy
```

Install the Amazon CloudWatch Agent on the EC2 instance.

Copy this repo file to the CloudWatch Agent config location:

```text
aws/cloudwatch-agent-windows.json
```

Important: edit `file_path` if your published API is not under:

```text
C:\inetpub\wwwroot\BankAccountServices\Logs\log*.txt
```

Start the agent with PowerShell as Administrator:

```powershell
& "C:\Program Files\Amazon\AmazonCloudWatchAgent\amazon-cloudwatch-agent-ctl.ps1" -a fetch-config -m ec2 -c file:C:\path\to\cloudwatch-agent-windows.json -s
```

## 4. Test the logs

Call the API from Swagger or curl. Then open CloudWatch:

```text
CloudWatch > Logs > Log groups > /bank-account-services/api
```

You should see a stream like:

```text
i-xxxxxxxxxxxxxxxxx/application
```

Search for:

```text
HTTP
ERR
```

## 5. Create a metric from errors

In the log group `/bank-account-services/api`, create a metric filter:

```text
Filter pattern: ERR
Metric namespace: BankAccountServices/API
Metric name: ApplicationErrors
Metric value: 1
Default value: 0
```

Then create an alarm:

```text
Metric: BankAccountServices/API > ApplicationErrors
Condition: >= 1 for 1 datapoint within 5 minutes
Action: send notification to an SNS topic
```

## 6. Learn the AWS pieces in this order

1. CloudWatch Logs log group
2. Log stream
3. CloudWatch Agent
4. Metric filter
5. Alarm
6. Dashboard
7. ALB metrics: `RequestCount`, `TargetResponseTime`, `HTTPCode_Target_5XX_Count`

For this project, the most useful dashboard widgets are:

- ALB request count
- ALB 5xx errors
- ALB target response time
- EC2 CPU
- EC2 memory
- EC2 disk free space
- `ApplicationErrors` custom metric
