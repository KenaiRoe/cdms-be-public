﻿--Watertemp 3
update datasets set Config = '{"DataEntryPage":{"HiddenFields":["ActivityDate"],"ShowFields":["Timezone"]},"ActivitiesPage":{"ShowFields":["Location.Label","headerdata.FieldActivityType","Description","User.Fullname"]}}' 
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'WaterTemp' );

--Water quality 6
update datasets set Config = '{"DataEntryPage":{},"ActivitiesPage":{"ShowFields":["Description", "Location.Label","headerdata.DataType","User.Fullname"]}}' 
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'WaterQuality' );

--Harvest/Creel
update datasets set Config = 
'{"DataEntryPage":{"HiddenFields":["Instrument","addNewRow"],"ShowFields":["Surveyor","addSection","addInterview","addFisherman","addAnotherFish"]},"ActivitiesPage":{"ShowFields":["ActivityDate","headerdata.TimeStart","Location.Label"]}}'
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'CreelSurvey' );

--Appraisal 4
update datasets set Config =
 '{"RestrictRoles":["DECD"],"ActivitiesPage":{"Route":"appraisals","ShowFields":["Allotment","AllotmentStatus"]},"DataEntryPage":{"HiddenFields":["Location","ActivityDate","Instrument","QA"]}}' 
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'Appraisal' );

--CrppContracts 16
update datasets set Config = 
'{"RestrictRoles":["CRPP"],"ActivitiesPage": {"Route": "crpp", "ShowFields":["headerdata.Allotment","headerdata.AllotmentStatus"]}, "DataEntryPage": {"HiddenFields": ["Location","ActivityDate","QA","Instrument"]}}'
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'CrppContracts' );

--FishScales 10
update datasets set Config = '{"DataEntryPage":{"HiddenFields":["Instrument","Location"]},"ActivitiesPage":{"ShowFields":["headerdata.RunYear","Location.Label","User.Fullname"]}}' 
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'FishScales' );

--Metrics 17
update datasets set Config = '{"DataEntryPage":{"HiddenFields":["Instrument","ActivityDate"]},"ActivitiesPage":{"ShowFields":["headerdata.YearReported","Location.Label"]}}' 
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'Metrics' );

--Benthic 21
update datasets set Config = '{"DataEntryPage": {"HiddenFields": ["ActivityDate","Instrument"], "ShowFields": ["SampleDate"]}}' 
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'Benthic' );

--Drift 22
update datasets set Config = '{"DataEntryPage": {"HiddenFields": ["ActivityDate","Instrument"], "ShowFields": ["SampleDate"]}}' 
where id in (select d.Id from Datasets d
 join DataStores ds on d.DatastoreId = ds.Id
where ds.TablePrefix = 'Drift' );

--StreamNet
update dbo.[Datasets] set Config = '{"DataEntryPage": {"HiddenFields": ["Instrument"]}}' where ProjectId = 9999 and Name like 'StreamNet%'