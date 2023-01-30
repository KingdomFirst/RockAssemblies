![Kingdom First Solutions](Images/KFSBanner.jpg)

# Microsoft 365 Utilities
<div style="background-color:#ffc107;padding:10px">Our Exchange Online integrations work with Microsoft's EWS api. The current process for utilizing this api requires setup and configuration of specific api credentials. See the <a href="#setup-ews-api-credentials">Setup EWS API Credentials</a> section for details.</div><br/>

## Summary
This package contains a suite of utilities for use with Microsoft 365. Our hope is to grow the list of utilities in this suite as churches' needs warrant. Have an idea for a new utility? Contact us today and let's start talking.

## Included Utilities
- EWS Calendar Items Shortcode ( Exchange Online )
- Launch Workflow From EWS Account Job ( Exchange Online )

## Quick Links
- [Setup EWS API credentials](#setup-ews-api-credentials)
- [EWS Calendar Items Shortcode](#ews-calendar-items-shortcode)
- [Launch Workflow From EWS Account Job](#launch-workflow-from-ews-account-job)

## Setup EWS API credentials
Our Exchange Online integrations work with Microsoft's EWS api. The current process for utilizing this api requires setup and configuration of specific api credentials. This setup and cofiguration is a Microsoft process and can change periodically. For that reason, we are offering only generalized instructions that point to more detailed instructions from Microsoft. If you need help setting up these credentials, contact us at sales@kingdomfirstsolutions.com.
  1. **Register Application In Azure Active Directory**: The EWS api requires credentials from an application registered on the same Azure Active Directory domain as the Exchange Online account(s) it will be accessing.<br/> 
  [https://learn.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth#register-your-application](https://learn.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth#register-your-application)
  2. **Configure Application In Azure**: Once the registered application has been created, it needs to be configured for app-only authentication.<br/> 
  [https://learn.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth#configure-for-app-only-authentication](https://learn.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth#configure-for-app-only-authentication)
  3. **Limit Application Mailbox Access**: The EWS api requires the application impersonate an existing Microsoft 365 user when accessing Online Exchange. The information the application can access is determined by the permissions of the impersonated user. The configuration recommoneded by Microsoft in the previous step gives your application admin access to impersonate any user in your Microsft 365 domain. For better security, we highly recommend you limit which users your application can impersonate by creating ApplicationAccessPolicies.<br/> 
  [https://learn.microsoft.com/en-us/graph/auth-limit-mailbox-access#configure-applicationaccesspolicy](https://learn.microsoft.com/en-us/graph/auth-limit-mailbox-access#configure-applicationaccesspolicy)

## EWS Calendar Items Shortcode
This shortcode can pull calendar items from any configured Microsoft 365 Online Exchange user/mailbox.

### Installation
The new shortcode is automatically added to existing shortcodes and ready to use once the plugin is installed.
![Shortcode List](Images/ShortcodeInList.png)

### Usage
The shortcode is called using the name "ewscalendaritems" with 9 possible parameters. It returns a CalendarItems object that contains a list of calendar items retrieved from the calendarmailbox.

#### Example

~~~
{% assign applicationid = 'Global' | Attribute:'rocks.kfs.EWSAppApplicationId' %}
{% assign tenantid = 'Global' | Attribute:'rocks.kfs.EWSAppTenantId' %}
{% assign appsecret = 'Global' | Attribute:'rocks.kfs.EWSAppSecret', 'RawValue' %}
{[ ewscalendaritems applicationid:'{{ applicationid }}' tenantid:'{{ tenantid }}' appsecret:'{{ appsecret }}' calendarmailbox:'example@kingdomfirstsolutions.com' impersonate:'user@kingdomfirstsolutions.com' ]}
    {% for calItem in CalendarItems %}
        {{ calItem.Subject }}
        {{ calItem.TextBody }}
        {{ calItem.Location }}
        {{ calItem.Start }}
        {{ calItem.End }}
    {% endfor %}
 {[ endewscalendaritems ]}
~~~
#### Parameters

- **applicationid** (REQUIRED) The Application (client) ID of the Azure registered application to be used. This value can be either encrypted or non-encrypted. It is recommended this value be stored in the global attribute EWS Azure Application ID that is installed with this plugin and passed from the attribute value directly to the shortcode for higher security. 
- **tenantid** (REQUIRED) The Directory (tenant) ID of the Azure registered application to be used. This value can be either encrypted or non-encrypted. It is recommended this value be stored in the global attribute EWS Azure Tenant ID that is installed with this plugin and passed from the attribute value directly to the shortcode for higher security. 
- **appsecret** (REQUIRED) The Secret Value for the Azure registered application to be used. This value can be either encrypted or non-encrypted. It is recommended this value be stored in the global attribute EWS Azure Secret that is installed with this plugin and passed from the attribute value directly to the shortcode for higher security. 
- **calendarmailbox** (REQUIRED) The address of the mailbox for the target calendar.
- **impersonate** (OPTIONAL) The Microsoft 365 user account to impersonate that has access to the calendarmailbox. If not provided, the calendarmailbox address will be used.
- **serverurl** (OPTIONAL) The url for the Microsoft Exchange server. Default: https://outlook.office365.com/EWS/Exchange.asmx. The default url should be used for all typical implementations.
- **order** (OPTIONAL) Changes the ordering of the returned items based on their Start value. By default items are ordered by Start ascending. Set value to 'desc' will cause the results to be orded by Start descending.
- **daysback** (OPTIONAL) The number of days to look back to find calendar items. Default: 0.
- **daysforward** (OPTIONAL) The number of days to look forward to find calendar items. Default: 7.

#### Available Calendar Item Fields

- **Subject** The subject.
- **Body** The full body.
- **TextBody** The plain text representation of the body.
- **Location** The name of the location.
- **Start** The start DateTime. (Timezone is based on the impersonated Exchange user's settings.)
- **End** The end DateTime. (Timezone is based on the impersonated Exchange user's settings.)
- **DisplayTo** A text summarization of the To recipients.
- **DisplayCc** A text summarization of the CC recipients.
- **IsRecurring** A boolean indicating if the calendar item is part of a recurring series.
<div style="page-break-after: always;"></div>

## Launch Workflow From EWS Account Job
Launch workflows by polling a Microsoft Exchange Online mailbox.

### Configuration

![JobSettings](Images/LaunchEWSWorkflowJobSettings.png)
<div style="page-break-after: always;"></div>
<style>
  table {
    background-color: rgba(220, 220, 220, 0.4);
  }
  th {
    display: none;
  }
</style>

### Job Settings
| | |
| --- | ---- |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">1</span> | **Application Id** The Application (client) ID of the Azure registered application to use. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">2</span> | **Tenant Id** The Directory (tenant) ID of the Azure registered application to use. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">3</span> | **Application Secret** The Secret Value of the Azure registered application to use. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">4</span> | **Email Address** The target Microsoft 365 Exchange Online mailbox. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">5</span> | **Impersonate User** The email address of the Microsoft 365 user to impersonate. NOTE: The Impersonate User must have permissions to access the inbox provided in the "Email Address" setting. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">6</span> | **Server Url** The URL for the Microsoft EWS api. NOTE: Default value should be used for standard use cases. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">7</span> | **Max Emails** The maximum number of emails to process each time the job runs. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">8</span> | **Launch Workflows with** The types of emails that should be processed from the inbox. If none are selected, any items of any type imported from the inbox will be processed. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">9</span> | **Mark Processed Emails by** How to mark the emails within the inbox once they are processed. Options work based on permissions. If multiple options are selected, they will be processed in the order presented and applied as permitted. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">10</span> | **One Workflow Per Conversation** Workflows are tagged with the Conversation identifier from the exchange item. Use this setting to control whether multiple items (emails) within a communication (i.e. replies, forwards, etc.) should continue to generate new workflows. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">11</span> | **Workflow Type** The type of Workflow to create for each processed mailbox item. |
| <span style="width: 3em; height: 3em; line-height: 3em; background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">12</span> | **Workflow Attributes** Map specific data from the mailbox item to existing Workflow attributes when creating the workflow. NOTE: The string value provided must match the Attribute Key of the target attribute. |