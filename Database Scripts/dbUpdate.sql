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

UPDATE SWITCH_STATION
SET SWITCH_STATION.BATTERY_CAPACITY = (
    SELECT COUNT(BATTERY_ID)
    FROM BATTERY
    WHERE BATTERY."switchStationStationId" = SWITCH_STATION.STATION_ID
);

UPDATE SWITCH_STATION
SET SWITCH_STATION.AVAILABLE_BATTERY_COUNT = (
    SELECT COUNT(BATTERY_ID)
    FROM BATTERY
    WHERE BATTERY."switchStationStationId" = SWITCH_STATION.STATION_ID and BATTERY.AVAILABLE_STATUS = 1
);

UPDATE SWITCH_STATION
SET STATION_NAME = '换电站 ' || TO_CHAR(STATION_ID)
WHERE 91 <= STATION_ID AND STATION_ID <= 134 OR STATION_ID < 70;

UPDATE MAINTENANCE_ITEM
SET NOTE =
    CASE
        WHEN REGEXP_LIKE(NOTE, '[a-zA-Z]') THEN
            CASE ROUND(DBMS_RANDOM.VALUE(1, 25))
                WHEN 1 THEN '轮胎需要更换，请检查。'
                WHEN 2 THEN '发动机异响，需要检修。'
                WHEN 3 THEN '请检查刹车系统。'
                WHEN 4 THEN '换机油和滤清器。'
                WHEN 5 THEN '灯光故障，需要修复。'
                WHEN 6 THEN '电池电量不足，充电问题。'
                WHEN 7 THEN '空调制冷效果不佳。'
                WHEN 8 THEN '损坏的车身部件需要更换。'
                WHEN 9 THEN '希望进行全面的车辆检查。'
                WHEN 10 THEN '发动机冷却液需要更换。'
                WHEN 11 THEN '请检查变速箱问题。'
                WHEN 12 THEN '空气滤清器需要清洗或更换。'
                WHEN 13 THEN '轮胎平衡问题，请修正。'
                WHEN 14 THEN '需要车辆漆面修复。'
                WHEN 15 THEN '电池终端腐蚀，请处理。'
                WHEN 16 THEN '车辆悬挂问题，需要修理。'
                WHEN 17 THEN '车辆底盘噪音问题。'
                WHEN 18 THEN '需要更换刹车片和刹车盘。'
                WHEN 19 THEN '车辆漏油，请检查。'
                WHEN 20 THEN '燃油滤清器需要更换。'
                WHEN 21 THEN '驾驶员座椅磨损，需要维修。'
                WHEN 22 THEN '需要车辆定期保养服务。'
                WHEN 23 THEN '电池充电器故障，需要修复。'
                WHEN 24 THEN '车辆喷漆工作需维护。'
                WHEN 25 THEN '发动机缺乏动力，请诊断。'
            END
    END
WHERE REGEXP_LIKE(NOTE, '[a-zA-Z]');

-- 随机替换包含英文的标题
UPDATE MAINTENANCE_ITEM
SET TITLE =
    CASE
        WHEN REGEXP_LIKE(TITLE, '[a-zA-Z]') THEN
            CASE ROUND(DBMS_RANDOM.VALUE(1, 7))
                WHEN 1 THEN '保险杠更换'
                WHEN 2 THEN '复合维修服务'
                WHEN 3 THEN '冷却液添加'
                WHEN 4 THEN '电动机保修'
                WHEN 5 THEN '清洗车辆'
                WHEN 6 THEN '轮胎更换'
                WHEN 7 THEN '其他英文标题'
            END
    END
WHERE REGEXP_LIKE(TITLE, '[a-zA-Z]');

UPDATE MAINTENANCE_ITEM
SET "Evaluation" =
    CASE
        WHEN REGEXP_LIKE("Evaluation", '[a-zA-Z]') THEN
            CASE ROUND(DBMS_RANDOM.VALUE(1, 40))
                WHEN 1 THEN '服务非常周到，印象深刻。'
                WHEN 2 THEN '员工态度很好，让人感到舒适。'
                WHEN 3 THEN '服务效率高，快速解决问题。'
                WHEN 4 THEN '专业水平一流，值得信赖。'
                WHEN 5 THEN '随时愿意提供帮助，感激不尽。'
                WHEN 6 THEN '服务质量卓越，非常出色。'
                WHEN 7 THEN '细致入微，服务体验棒极了。'
                WHEN 8 THEN '回应速度快，让人满意。'
                WHEN 9 THEN '服务态度亲切，让人感到温暖。'
                WHEN 10 THEN '提供的服务非常专业可靠。'
                WHEN 11 THEN '处理问题迅速，令人满意。'
                WHEN 12 THEN '帮助解决困难，感到非常感激。'
                WHEN 13 THEN '服务周到，让人感到放心。'
                WHEN 14 THEN '服务水平高，值得赞扬。'
                WHEN 15 THEN '回答问题详细，协助很大。'
                WHEN 16 THEN '提供的服务让人印象深刻。'
                WHEN 17 THEN '态度友好，服务态度好极了。'
                WHEN 18 THEN '提供的服务效率很高。'
                WHEN 19 THEN '细心关注细节，令人满意。'
                WHEN 20 THEN '专业度高，非常可信赖。'
                WHEN 21 THEN '处理事务非常迅速，感谢。'
                WHEN 22 THEN '服务周到，让人感到安心。'
                WHEN 23 THEN '服务水平令人印象深刻。'
                WHEN 24 THEN '回应速度快，非常感谢。'
                WHEN 25 THEN '提供的服务非常专业。'
                WHEN 26 THEN '处理问题迅速，效率很高。'
                WHEN 27 THEN '随时为客户着想，感到高兴。'
                WHEN 28 THEN '服务态度亲切，非常感激。'
                WHEN 29 THEN '提供的服务让人放心。'
                WHEN 30 THEN '专业水平出色，非常满意。'
                WHEN 31 THEN '服务周到，令人满意。'
                WHEN 32 THEN '回答问题详细，帮助很大。'
                WHEN 33 THEN '提供的服务值得称赞。'
                WHEN 34 THEN '处理事务非常迅速，印象深刻。'
                WHEN 35 THEN '细心关注细节，协助很多。'
                WHEN 36 THEN '服务效率高，非常可靠。'
                WHEN 37 THEN '专业度让人信服，感谢。'
                WHEN 38 THEN '提供的服务态度友好。'
                WHEN 39 THEN '随时愿意帮助，让人感激。'
                WHEN 40 THEN '服务水平非常高，非常出色。'
            END
    END
WHERE REGEXP_LIKE("Evaluation", '[a-zA-Z]');

UPDATE SWITCH_LOG
SET SCORE = ROUND(SCORE);

UPDATE SWITCH_LOG
SET "Evaluation" = NULL
WHERE SCORE = -1;

UPDATE SWITCH_LOG
SET "Evaluation" =
    CASE
        WHEN REGEXP_LIKE("Evaluation", '[a-zA-Z]') THEN
            CASE ROUND(DBMS_RANDOM.VALUE(1, 40))
                WHEN 1 THEN '服务非常周到，印象深刻。'
                WHEN 2 THEN '员工态度很好，让人感到舒适。'
                WHEN 3 THEN '服务效率高，快速解决问题。'
                WHEN 4 THEN '专业水平一流，值得信赖。'
                WHEN 5 THEN '随时愿意提供帮助，感激不尽。'
                WHEN 6 THEN '服务质量卓越，非常出色。'
                WHEN 7 THEN '细致入微，服务体验棒极了。'
                WHEN 8 THEN '回应速度快，让人满意。'
                WHEN 9 THEN '服务态度亲切，让人感到温暖。'
                WHEN 10 THEN '提供的服务非常专业可靠。'
                WHEN 11 THEN '处理问题迅速，令人满意。'
                WHEN 12 THEN '帮助解决困难，感到非常感激。'
                WHEN 13 THEN '服务周到，让人感到放心。'
                WHEN 14 THEN '服务水平高，值得赞扬。'
                WHEN 15 THEN '回答问题详细，协助很大。'
                WHEN 16 THEN '提供的服务让人印象深刻。'
                WHEN 17 THEN '态度友好，服务态度好极了。'
                WHEN 18 THEN '提供的服务效率很高。'
                WHEN 19 THEN '细心关注细节，令人满意。'
                WHEN 20 THEN '专业度高，非常可信赖。'
                WHEN 21 THEN '处理事务非常迅速，感谢。'
                WHEN 22 THEN '服务周到，让人感到安心。'
                WHEN 23 THEN '服务水平令人印象深刻。'
                WHEN 24 THEN '回应速度快，非常感谢。'
                WHEN 25 THEN '提供的服务非常专业。'
                WHEN 26 THEN '处理问题迅速，效率很高。'
                WHEN 27 THEN '随时为客户着想，感到高兴。'
                WHEN 28 THEN '服务态度亲切，非常感激。'
                WHEN 29 THEN '提供的服务让人放心。'
                WHEN 30 THEN '专业水平出色，非常满意。'
                WHEN 31 THEN '服务周到，令人满意。'
                WHEN 32 THEN '回答问题详细，帮助很大。'
                WHEN 33 THEN '提供的服务值得称赞。'
                WHEN 34 THEN '处理事务非常迅速，印象深刻。'
                WHEN 35 THEN '细心关注细节，协助很多。'
                WHEN 36 THEN '服务效率高，非常可靠。'
                WHEN 37 THEN '专业度让人信服，感谢。'
                WHEN 38 THEN '提供的服务态度友好。'
                WHEN 39 THEN '随时愿意帮助，让人感激。'
                WHEN 40 THEN '服务水平非常高，非常出色。'
            END
    END
WHERE REGEXP_LIKE("Evaluation", '[a-zA-Z]');

UPDATE MAINTENANCE_ITEM
SET SCORE = ROUND(SCORE * 2) / 2;

UPDATE MAINTENANCE_ITEM
SET SCORE = DBMS_RANDOM.VALUE(3, 5)
WHERE ORDER_STATUS = 4 AND SCORE = -1;

UPDATE MAINTENANCE_ITEM
SET "Evaluation" = null 
WHERE ORDER_STATUS != 4;

UPDATE MAINTENANCE_ITEM
SET SCORE = -1
WHERE ORDER_STATUS != 4;

UPDATE SWITCH_LOG
SET SCORE = -1, "Evaluation" = NULL
WHERE "switchRequestId" IN (SELECT SWITCH_REQUEST_ID FROM SWITCH_REQUEST WHERE "RequestStatus" != 4);

UPDATE SWITCH_REQUEST
SET NOTES =
    CASE
        WHEN DBMS_RANDOM.VALUE <= 0.8 THEN
            CASE ROUND(DBMS_RANDOM.VALUE(1, 40))
                WHEN 1 THEN '请检查电池容量。'
                WHEN 2 THEN '需要更换损坏的电池。'
                WHEN 3 THEN '希望取出电池充电。'
                WHEN 4 THEN '请提供更多电池。'
                WHEN 5 THEN '电池外壳有损坏，需要处理。'
                WHEN 6 THEN '希望提前预约换电。'
                WHEN 7 THEN '电池号码：ABC123456。'
                WHEN 8 THEN '请尽快送达电池。'
                WHEN 9 THEN '检查电池的充电状态。'
                WHEN 10 THEN '换电地点已更改。'
                WHEN 11 THEN '希望电池质量较新。'
                WHEN 12 THEN '需要更多电池充电插座。'
                WHEN 13 THEN '换电时间请在晚上7点之后。'
                WHEN 14 THEN '电池需求：2块。'
                WHEN 15 THEN '需要一辆电动车更换电池。'
                WHEN 16 THEN '希望电池标记日期在半年内。'
                WHEN 17 THEN '需要电池安装服务。'
                WHEN 18 THEN '换电地址：XX街XX号。'
                WHEN 19 THEN '需要更多电池充电站点。'
                WHEN 20 THEN '希望在下午2点之前完成换电。'
                WHEN 21 THEN '请尽量选择绿色电池。'
                WHEN 22 THEN '换电费用需要发票。'
                WHEN 23 THEN '电池容量不足，请检查。'
                WHEN 24 THEN '希望换电时提供电池状态报告。'
                WHEN 25 THEN '电池类型：锂离子。'
                WHEN 26 THEN '需要电池交付上门。'
                WHEN 27 THEN '电池容量需求：50%以上。'
                WHEN 28 THEN '希望电池生产日期不早于一年前。'
                WHEN 29 THEN '请在工作时间内完成换电。'
                WHEN 30 THEN '电池质量需满足标准。'
                WHEN 31 THEN '换电站点已关闭，请指示。'
                WHEN 32 THEN '电池充电速度较慢，请注意。'
                WHEN 33 THEN '请提供电池使用说明。'
                WHEN 34 THEN '希望电池包含保修信息。'
                WHEN 35 THEN '需要将旧电池回收。'
                WHEN 36 THEN '换电时间需要提前预约。'
                WHEN 37 THEN '电池型号：XX型号。'
                WHEN 38 THEN '换电位置：XX楼XX号。'
                WHEN 39 THEN '电池续航不足，请检查。'
                WHEN 40 THEN '请检查电池连接状态。'
            END
    END;

DELETE FROM SWITCH_LOG
WHERE "switchRequestId" IN (
SELECT SWITCH_REQUEST_ID
    FROM SWITCH_REQUEST
    WHERE "EmployeeId" IN (
        SELECT EMPLOYEE_ID
        FROM EMPLOYEE
        WHERE EMPLOYEE.POSITIONS = 2
    )
);

DELETE FROM SWITCH_REQUEST
WHERE "EmployeeId" IN (
    SELECT EMPLOYEE_ID
    FROM EMPLOYEE
    WHERE EMPLOYEE.POSITIONS = 2
);