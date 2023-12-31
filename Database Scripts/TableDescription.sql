
Ref: ADMINISTRATOR.ADMIN_ID < NEWS.ADMIN_ID
Table ADMINISTRATOR {
  ADMIN_ID DECIMAL(19, 0) [NOT NULL]
  ACCOUNT_SERIAL VARCHAR(2000)
  EMAIL VARCHAR(2000)
  PASSWORD VARCHAR(50) [NOT NULL]
}

Table BATTERY {
  BATTERY_ID DECIMAL(19, 0) [NOT NULL]
  AVAILABLE_STATUS BIGINT
  CURRENT_CAPACITY DOUBLEPRECISION [NOT NULL]
  CURR_CHARGE_TIMES BIGINT [NOT NULL]
  MANUFACTURING_DATE TIMESTAMP(7) [NOT NULL]
  STATION_ID DECIMAL(19, 0)
  BATTERY_TYPE_ID DECIMAL(19, 0) [NOT NULL]
}

Table BATTERY_TYPE {
  BATTERY_TYPE_ID DECIMAL(19, 0) [NOT NULL]
  MAX_CHARGE_TIEMS BIGINT [NOT NULL]
  TOTAL_CAPACITY VARCHAR(2000)
  NAME VARCHAR(2000)
}

Table EMPLOYEE {
  EMPLOYEE_ID DECIMAL(19, 0) [NOT NULL]
  ACCOUNT_SERIAL VARCHAR(2000)
  EMAIL VARCHAR(2000)
  USERNAME VARCHAR(30)
  PASSWORD VARCHAR(50) [NOT NULL, DEFAULT: `N'123456'`]
  PROFILE_PHOTO BYTEA
  CREATE_TIME TIMESTAMP(7) [NOT NULL]
  PHONE_NUMBER VARCHAR(20)
  IDENTITYNUMBER VARCHAR(50)
  NAME VARCHAR(30)
  GENDER VARCHAR(3)
  POSITIONS BIGINT [NOT NULL]
  SALARY BIGINT [NOT NULL]
  STATION_ID DECIMAL(19, 0)
}

Table EMPLOYEE_MAINTENANCEITEM {
  EMPLOYEE_ID DECIMAL(19, 0) [NOT NULL]
  MAINTENANCE_ID DECIMAL(19, 0) [NOT NULL]
}

Table KPI {
  KPI_ID DECIMAL(19, 0) [NOT NULL]
  TOTAL_PERFORMANCE DOUBLEPRECISION [NOT NULL]
  SERVICE_FREQUENCY BIGINT [NOT NULL]
  SCORE DOUBLEPRECISION [NOT NULL]
  EMPLOYEE_ID DECIMAL(19, 0) [NOT NULL]
}

Table MAINTENANCE_ITEM {
  MAINTENANCE_ITEM_ID DECIMAL(19, 0) [NOT NULL]
  MAINTENANCE_LOCATION VARCHAR(50) [NOT NULL]
  NOTE VARCHAR(2000)
  TITLE VARCHAR(2000)
  LONGITUDE DOUBLEPRECISION [NOT NULL]
  LATITUDE DOUBLEPRECISION [NOT NULL]
  SERVICE_TIME TIMESTAMP(7)
  ORDER_SUBMISSION_TIME TIMESTAMP(6) [NOT NULL]
  APPOINT_TIME TIMESTAMP(7) [NOT NULL]
  ORDER_STATUS BIGINT [NOT NULL]
  SCORE DOUBLEPRECISION [NOT NULL]
  EVALUATION VARCHAR(2000)
  VEHICLE_ID DECIMAL(19, 0) [NOT NULL]
}

Table NEWS {
  ANNOUNCEMENT_ID DECIMAL(19, 0) [NOT NULL]
  PUBLISH_TIME TIMESTAMP(7) [NOT NULL]
  PUBLISH_POS BIGINT [NOT NULL]
  TITLE VARCHAR(50)
  CONTENTS TEXT
  LIKES BIGINT [NOT NULL]
  VIEW_COUNT BIGINT [NOT NULL]
  ADMIN_ID DECIMAL(19, 0) [NOT NULL]
}

Table OWNERPOS {
  OWNER_ID DECIMAL(19, 0) [NOT NULL]
  ADDRESS VARCHAR(2000) [NOT NULL]
}

Table SWITCH_LOG {
  SWITCH_SERVICE_ID DECIMAL(19, 0) [NOT NULL]
  SWITCH_TIME TIMESTAMP(7) [NOT NULL]
  SCORE DOUBLEPRECISION [NOT NULL]
  EVALUATION VARCHAR(2000)
  SERVICE_FEE REAL [NOT NULL]
  BATTERY_ON_ID DECIMAL(19, 0) [NOT NULL]
  BATTERY_OFF_IF DECIMAL(19, 0) [NOT NULL]
  SWITCH_REQUEST_ID DECIMAL(19, 0) [NOT NULL]
}

Table SWITCH_REQUEST {
  SWITCH_REQUEST_ID DECIMAL(19, 0) [NOT NULL]
  SWITCH_TYPE BIGINT [NOT NULL]
  REQUEST_TIME TIMESTAMP(7) [NOT NULL]
  POSITION VARCHAR(50)
  LONGITUDE DOUBLEPRECISION [NOT NULL]
  LATITUDE DOUBLEPRECISION [NOT NULL]
  NOTES VARCHAR(255)
  DATE TIMESTAMP(7) [NOT NULL]
  REQUEST_STATUS BIGINT [NOT NULL]
  EMPLOYEE_ID DECIMAL(19, 0) [NOT NULL]
  VEHICLE_ID DECIMAL(19, 0) [NOT NULL]
  BATTERY_TYPE_ID DECIMAL(19, 0) [NOT NULL]
  PERIOD BIGINT
}

Table SWITCH_STATION {
  STATION_ID DECIMAL(19, 0) [NOT NULL]
  STATION_NAME VARCHAR(50)
  SERVICE_FEE REAL [NOT NULL]
  ELECTRICITY_FEE REAL [NOT NULL]
  PARKING_FEE VARCHAR(2000) [NOT NULL]
  QUEUE_LENGTH BIGINT [NOT NULL]
  LONGITUDE DOUBLEPRECISION [NOT NULL]
  LATITUDE DOUBLEPRECISION [NOT NULL]
  FAILURE_STATUS SMALLINT [NOT NULL]
  BATTERY_CAPACITY BIGINT [NOT NULL]
  AVAILABLE_BATTERY_COUNT BIGINT [NOT NULL]
  CITY VARCHAR(2000)
  TAGS VARCHAR(2000)
  ADDRESS VARCHAR(2000)
  TIMESPAN VARCHAR(2000)
}

Table VEHICLE {
  VEHICLE_ID DECIMAL(19, 0) [NOT NULL]
  PURCHASE_DATE TIMESTAMP(7) [NOT NULL]
  BATTERY_ID DECIMAL(19, 0) [NOT NULL]
  PLATE_NUMBER VARCHAR(2000)
  MILEAGE BIGINT [NOT NULL]
  TEMPERATURE BIGINT [NOT NULL]
  WARRANTY TIMESTAMP(7) [NOT NULL]
  OWNER_ID DECIMAL(19, 0) [NOT NULL]
  VEHICLE_MODEL_ID DECIMAL(19, 0) [NOT NULL]
}

Table VEHICLE_OWNER {
  OWNER_ID DECIMAL(19, 0) [NOT NULL]
  ACCOUNT_SERIAL VARCHAR(2000)
  USERNAME VARCHAR(50)
  EMAIL VARCHAR(50)
  PASSWORD VARCHAR(50) [NOT NULL]
  PROFILE_PHOTO BYTEA
  CREATE_TIME TIMESTAMP(7) [NOT NULL]
  PHONE_NUMBER VARCHAR(20) [NOT NULL]
  GENDER VARCHAR(3)
  BIRTHDAY TIMESTAMP(7)
}

Table VEHICLE_PARAM {
  VEHICLE_MODEL DECIMAL(19, 0) [NOT NULL]
  ModelNAME VARCHAR(2000) [NOT NULL]
  TRANSMISSION VARCHAR(50) [NOT NULL]
  SERVICE_TERM TIMESTAMP(7) [NOT NULL]
  MANUFACTURER VARCHAR(50) [NOT NULL]
  MAX_SPEED BIGINT [NOT NULL]
  SINP BYTEA
}

Table __EFMigrationsHistory {
  MigrationId VARCHAR(150) [NOT NULL]
  ProductVersion VARCHAR(32) [NOT NULL]
}




Ref: "SWITCH_LOG"."SWITCH_REQUEST_ID" > "SWITCH_REQUEST"."SWITCH_REQUEST_ID"

Ref: "KPI"."EMPLOYEE_ID" - "EMPLOYEE"."EMPLOYEE_ID"

Ref: "VEHICLE_OWNER"."OWNER_ID" < "VEHICLE"."OWNER_ID"

Ref: "VEHICLE_PARAM"."VEHICLE_MODEL" < "VEHICLE"."VEHICLE_MODEL_ID"

Ref: "VEHICLE"."VEHICLE_ID" < "SWITCH_REQUEST"."VEHICLE_ID"

Ref: "BATTERY_TYPE"."BATTERY_TYPE_ID" < "SWITCH_REQUEST"."BATTERY_TYPE_ID"

Ref: "BATTERY_TYPE"."BATTERY_TYPE_ID" < "BATTERY"."BATTERY_TYPE_ID"

Ref: "SWITCH_STATION"."STATION_ID" < "BATTERY"."STATION_ID"

Ref: "BATTERY"."BATTERY_ID" < "VEHICLE"."BATTERY_ID"

Ref: "VEHICLE_OWNER"."OWNER_ID" < "OWNERPOS"."OWNER_ID"

Ref: "VEHICLE"."VEHICLE_ID" < "MAINTENANCE_ITEM"."VEHICLE_ID"

Ref: "BATTERY"."BATTERY_ID" < "SWITCH_LOG"."BATTERY_ON_ID"

Ref: "BATTERY"."BATTERY_ID" < "SWITCH_LOG"."BATTERY_OFF_IF"

Ref: "SWITCH_STATION"."STATION_ID" < "EMPLOYEE"."STATION_ID"

Ref: "EMPLOYEE"."EMPLOYEE_ID" < "SWITCH_REQUEST"."EMPLOYEE_ID"

Ref: "EMPLOYEE"."EMPLOYEE_ID" < "EMPLOYEE_MAINTENANCEITEM"."EMPLOYEE_ID"

Ref: "MAINTENANCE_ITEM"."MAINTENANCE_ITEM_ID" < "EMPLOYEE_MAINTENANCEITEM"."MAINTENANCE_ID"