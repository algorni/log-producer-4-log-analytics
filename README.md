# Simple C# app that produce a logs entries to be collected by log analytics agent

This app will generate some concurrent logs with the following format / example:

>2020-06-01T13:05:46.394+00:00,Information,Application started. Press Ctrl+C to shut down.
2020-06-01T13:05:46.399+00:00,Information,Hosting environment: "Production"
2020-06-01T13:05:46.400+00:00,Information,Content root path: "/home/sysadmin/log-producer-4-log-analytics/log-producer-app/log-producer-app/bin/Debug/netcoreapp3.1/"
2020-06-01T13:05:46.405+00:00,Information,Stopping LogProducer.
2020-06-01T13:05:46.408+00:00,Information,LogProducer....
2020-06-01T13:05:48.869+00:00,Information,Starting LogProducer with Max Operation Time: 5000
2020-06-01T13:05:48.891+00:00,Information,b7ea14ff-a034-4256-aa26-126c7cd93a70,Begin Something
2020-06-01T13:05:49.490+00:00,Information,b7ea14ff-a034-4256-aa26-126c7cd93a70,Completed Something
2020-06-01T13:05:55.139+00:00,Information,0df7f16b-ba52-4eeb-afd9-7a4e5f471e65,Begin Something
2020-06-01T13:05:57.500+00:00,Information,0df7f16b-ba52-4eeb-afd9-7a4e5f471e65,Completed Something
2020-06-01T13:06:03.617+00:00,Information,fab02e1c-beba-44ff-ad1f-94b824629da3,Begin Something
2020-06-01T13:06:04.821+00:00,Information,fab02e1c-beba-44ff-ad1f-94b824629da3,Completed Something
2020-06-01T13:06:06.866+00:00,Information,ee9011d2-26dc-4977-8865-419ae36acf58,Begin Something
2020-06-01T13:06:08.143+00:00,Information,ee9011d2-26dc-4977-8865-419ae36acf58,Completed Something
2020-06-01T13:06:10.615+00:00,Information,89ec84da-95e8-4b36-b73c-054eef851c7d,Begin Something
2020-06-01T13:06:14.777+00:00,Information,89ec84da-95e8-4b36-b73c-054eef851c7d,Completed Something
2020-06-01T13:09:09.347+00:00,Information,Application is shutting down...
2020-06-01T13:09:10.152+00:00,Information,Application started. Press Ctrl+C to shut down.
2020-06-01T13:09:10.156+00:00,Information,Hosting environment: "Production"
2020-06-01T13:09:10.160+00:00,Information,Content root path: "/home/sysadmin/log-producer-4-log-analytics/log-producer-app/log-producer-app/bin/Debug/netcoreapp3.1/"
2020-06-01T13:09:10.165+00:00,Information,Stopping LogProducer.

The purpose of this sample app is to easly test out log analytics capabilities on ingesting custom logs and performing kusto query on top of it. 

## Setting Up the Environment

The step to achieve this is to deploy this application and running into a linux box, collect the logs and being able to explore the logs with Log Analytics are the following:

0. Create your Log Analytics Workspace.
1. Create a Linux Box on Azure.
2. Then installing the Azure Monitor agent (OMS Agent) as per the following instructions:
 https://docs.microsoft.com/azure/azure-monitor/platform/log-analytics-agent
 In case of your linux box is running in Azure you can just onboard the linux vm into your Log Analytics workspace
3. Install dotnet sdk in your linx box:
 https://docs.microsoft.com/dotnet/core/install/sdk?pivots=os-linux
4. Clone this repository and executing the app on your linux box
5. Enabling Custom logs on Log Analytics:
 https://docs.microsoft.com/en-gb/azure/azure-monitor/platform/data-sources-custom-logs

>Note: this app use serilog to stream out logs into rolling files and use the following format for timestamp:
> yyyy-MM-ddTHH:mm:ss.fffK that seems actually compatible with the list of time stamp handled byt Log Analytics Custom Logs

## Kusto Query

The following KUSTO query is an example of how easly you can obtain execution time of your operations


>ScriptLogs_CL
| parse RawData with TimeStamp ","  Level "," CorrelationId "," StartMessage
| where StartMessage contains "Begin"
| extend StartTime = todatetime(TimeStamp) 
| project StartTime, Level, CorrelationId, StartMessage
| join (
ScriptLogs_CL 
| parse RawData with TimeStamp "," Level "," CorrelationId "," EndMessage
| where EndMessage contains "Completed"
| extend EndTime = todatetime(TimeStamp) 
| project EndTime, Level, CorrelationId, EndMessage
) on CorrelationId
| extend Duration = EndTime - StartTime
| extend DurationMilliseconds = Duration / time(1ms)
| project StartTime, EndTime, Duration,DurationMilliseconds, CorrelationId, StartMessage, EndMessage
| summarize avg(DurationMilliseconds), percentiles(DurationMilliseconds, 50, 95)  by bin(StartTime, 1m)
