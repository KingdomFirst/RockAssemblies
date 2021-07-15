![KFSBanner (3)](https://user-images.githubusercontent.com/81330042/118855629-938f0f00-b89b-11eb-8b82-7496ebd61e08.jpg)



# Send Scheduled Group Communications
*Tested/Supported in Rock version:  8.0-12.0*   
*Created:  11/20/2018*  
*Updated:  6/22/2021*   
*Rock Shop Plugin: https://www.rockrms.com/Plugin/100*

# Send Scheduled Group Communications

## Summary

For groups that are not a Communication List, you use the communication button on the group members grid to communicate with group members. If you schedule the communication to send in the future, anyone added to the group after the communication is created will not receive the communication.

This plugin will add two jobs and two attribute matrixes that work together to allow you to schedule communications to groups. This method allows you to craft your communication in advance and it will be sent all the people who are members of the group at the time the communication is sent. **You will need Edit permissions to the Group to send communications.** Communications sent through these jobs will be saved to the person's history just like any other communication.



Quick Links:

- [What's New](#whats-new)
- [Configuring the Groups](#configuring-the-groups)
- [Configuring the Jobs](#configuring-the-jobs)
- [Sending a Communication](#sending-a-communication)



## What's New

The following new goodness will be added to your Rock install with this plugin:

- **New Job Type**: Send Scheduled Group Email
- **New Job Type**: Send Scheduled Group SMS
- **Attribute Matrix Template**: Scheduled Emails
- **Attribute Matrix Template**: Scheduled SMS Messages



## Configuring the Groups

You will need to add the two attribute matrixes to any group type you want to allow to schedule communications.

1. Go to Admin Tools > General Settings > Group Types and edit your chosen group type.
2. Under Group Attributes, add an attribute
3. You will need to create a separate attribute for SMS and Email. The only differences will be the names and which Attribute Matrix Template you select.

![SMS_Group_Attribute (1)](https://user-images.githubusercontent.com/81330042/123459868-76d9ab80-d5ac-11eb-89f8-a8045b16d93f.png)

> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;1&nbsp;&nbsp;</span> **Name** Enter a name for your attribute
>
> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;2&nbsp;&nbsp;</span> **Field Type** Choose Matrix
>
> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;3&nbsp;&nbsp;</span> **Attribute Matrix Template** Select the Scheduled SMS Messages or Scheduled Email



## Configuring the Jobs

You will need to create two system jobs to send the communications.

**The first time the jobs run, it will look back 24 hours to see if any groups had communications to send. Each subsequent run looks back to the time since the last successful run. Only create one email job and one SMS job. Creating multiple jobs of each type will result in messages being sent multiple times.**

1. Go to Admin Tools > System Settings > Jobs Administration
2. Add a new job

![Email_Job (2)](https://user-images.githubusercontent.com/81330042/123459947-8f49c600-d5ac-11eb-8fb0-3fa27aa8405e.png)

> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;1&nbsp;&nbsp;</span> **Name** Give your job a name
>
> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;2&nbsp;&nbsp;</span>**Cron Expression** Enter a cron expression for how often you want the job to run. Communications will be sent the next time the job runs after their scheduled send time.
>
> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;3&nbsp;&nbsp;</span>**Job Type** For an email job, choose "rocks.kfs.GroupScheduledEmail.Jobs.SendScheduledGroupEmail" For an SMS job, choose "rocks.kfs.GroupScheduledSMS.Jobs.SendScheduledGroupSMS"
>
> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;4&nbsp;&nbsp;</span>**Enabled Lava Commands** Choose which lava commands should be enabled for this job. If you would like to use any of these commands in your communication, they need to be enabled on the job.
>
> <span style="padding-left: 30px; margin-right: 10px; width: .8em;background: #d21919; border-radius: 100%; color: white; text-align: center; display: inline-block;">&nbsp;&nbsp;5&nbsp;&nbsp;</span>**Last Run Buffer** Use this setting to prevent messages from sending twice, too large a buffer may cause messages to be not sent. By default this job will send any communications that have been scheduled since the last run date time minus the last run duration seconds. Due to the way some server schedules run, it is possible to be a few seconds off and double send.



## Sending a Communication

1. In your Group Viewer, go to the group you want to send a communication

2. Click the Edit button on the group details

3. Under Group Attribute Values, click a plus sign for the communication type you want to create

![Group_Attributes (1)](https://user-images.githubusercontent.com/81330042/123460131-c9b36300-d5ac-11eb-9c1c-cb303788cbb3.png)

4. All the fields for creating a communication are required. You can use Lava in the message field for both SMS and email communications. Recurrence defaults to One Time but you can also choose Weekly, Biweekly, or Monthly (the same day of the month each month). The Person, Group Member and Organization entities are all available for lava use in both email and SMS.

![Create_Email (1)](https://user-images.githubusercontent.com/81330042/123460147-d041da80-d5ac-11eb-8e4b-1081c550560d.png)



![Create_SMS (1)](https://user-images.githubusercontent.com/81330042/123460161-d6d05200-d5ac-11eb-89b6-012c214f5f23.png)


