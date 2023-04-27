# GeeksTestTask
1) In order to import/export files correcly the program assumes you will give json file with next structure (without any extra spaces or new lines):
[{Id: "94a12fe6-8029-44c5-9430-ac7bbde6349d", Name: "Root Folder", ParentId: "ad803ae3-0ad7-4e1d-8e73-9e0fdc91970a"},{Id: "ce1fe9f9-0b75-47fa-bf1c-152b33c0c766", Name: "Root Folder", ParentId: "fa5d2420-3737-4373-b390-0e287418c7c8"}].
2) You will also have to add custom connection string in appsettings.json file in the "ConnectionStrings" section and set "DefaultConnection" value.
