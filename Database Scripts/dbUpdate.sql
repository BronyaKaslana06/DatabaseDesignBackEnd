UPDATE SWITCH_REQUEST SR
SET SR.RequestStatus = 
    CASE 
        WHEN SR.switchRequestId NOT IN (SELECT switchRequestId FROM SWITCH_LOG) THEN
            CASE 
                WHEN SR.SWITCH_TYPE = 1 THEN ROUND(DBMS_RANDOM.VALUE(1, 2))
                WHEN SR.SWITCH_TYPE = 2 THEN 2
                ELSE SR.RequestStatus
            END
        ELSE SR.RequestStatus
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
set AVAILABLE_STATUS=DBMS_RANDOM.VALUE(1, 6)
where BATTERY_ID in (
select t1.BATTERY_ID
from BATTERY t1
left join VEHICLE t2
on t1.BATTERY_ID=t2."BatteryId"
where t2.VEHICLE_ID is null
);
UPDATE Battery 
SET AVAILABLE_STATUS = 3
WHERE
	BATTERY_ID IN ( SELECT BATTERY_ID FROM VEHICLE WHERE "BatteryId" IS NOT NULL );

UPDATE EMPLOYEE
SET "switchStationStationId" = NULL
WHERE POSITIONS = 2;
