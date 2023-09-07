UPDATE SWITCH_REQUEST SR
SET SR."RequestStatus" = 
    CASE 
        WHEN SR."SWITCH_REQUEST_ID" NOT IN (SELECT "switchRequestId" FROM SWITCH_LOG) THEN
            CASE 
                WHEN SR.SWITCH_TYPE = 1 THEN ROUND(DBMS_RANDOM.VALUE(1, 2))
                WHEN SR.SWITCH_TYPE = 2 THEN 2
                ELSE SR."RequestStatus"
            END
        ELSE SR."RequestStatus"
    END;

update SWITCH_REQUEST
set "RequestStatus"=3
where SWITCH_REQUEST_ID in (
select t2.SWITCH_REQUEST_ID
from SWITCH_LOG t1
join SWITCH_REQUEST t2
on t1."switchRequestId"=t2.SWITCH_REQUEST_ID
where t1.SCORE<0
);
update SWITCH_REQUEST
set "RequestStatus"=4
where SWITCH_REQUEST_ID in (
select t2.SWITCH_REQUEST_ID
from SWITCH_LOG t1
join SWITCH_REQUEST t2
on t1."switchRequestId"=t2.SWITCH_REQUEST_ID
where t1.SCORE>=0
);


update BATTERY
set AVAILABLE_STATUS=DBMS_RANDOM.VALUE(1, 5)
where BATTERY_ID in (
select t1.BATTERY_ID
from BATTERY t1
left join VEHICLE t2
on t1.BATTERY_ID=t2."BatteryId"
where t2.VEHICLE_ID is null
);

UPDATE Battery 
SET AVAILABLE_STATUS = 3, "switchStationStationId"=null
WHERE
	BATTERY_ID IN ( SELECT "BatteryId" FROM VEHICLE WHERE "BatteryId" IS NOT NULL );
	
update BATTERY
set AVAILABLE_STATUS = 1
where BATTERY_ID not in ( SELECT "BatteryId" FROM VEHICLE WHERE "BatteryId" IS NOT NULL) and AVAILABLE_STATUS = 3;

delete from BATTERY
where AVAILABLE_STATUS!=3 and "switchStationStationId" is null;

UPDATE EMPLOYEE
SET "switchStationStationId" = NULL
WHERE POSITIONS = 2;

UPDATE SWITCH_STATION AS s
SET s.BATTERY_CAPACITY = (
    SELECT COUNT(BATTERY_ID)
    FROM BATTERY AS b
    WHERE b."switchStationStationId" = s.STATION_ID
);

UPDATE SWITCH_STATION AS s
SET s.AVAILABLE_BATTERY_COUNT = (
    SELECT COUNT(BATTERY_ID)
    FROM BATTERY AS b
    WHERE b."switchStationStationId" = s.STATION_ID and b.AVAILABLE_STATUS == 1
);