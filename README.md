# CalendarMerge

There might be an easier solution to this, but I got annoyed when trying to share my multiple calendars with my wife. I have my main calendar, and then any number of team sport calenders, or other imported caledners and just wanted to expose this as one calendar to my wife. So this small project was born. It is a simple Azure Function that based on the URL provided, will read from a json file in Azure Blob Storage via a URL that just has an array of URL's to iCals. These iCals will be merged together as one iCal Calendar. This URL can be imported right into any calendar app and now you have one Calendar which will stay updated from a number of other Calendars.

Lots of things to extend/update but for now, it is working for it's main purpose
