create table c###car.switch_station (
    station_id varchar2(50) default 'st0' primary key,
    station_name varchar2(50),
    queue_length int default 0 not null,
    service_fee number default 0 not null,
    electricity_fee number default 0 not null,
    longtitude number default 90.00 not null unique,
    latitude number default 90.00 not null unique,
    faliure_status char(3) default '否' check (faliure_status in ('是', '否')),
    battery_capacity int default 0 not null,
    available_battery_count int default 0
);

create table c###car.battery_type (
    battery_type_id varchar2(50) default 'tp0.00.0' primary key,
    max_charge_tiems int default 1000 not null,
    total_capacity varchar2(50) default '98.67kwh' not null
);

create table c###car.battery (
    battery_id varchar2(50) default 'bt_tl1_num0.0.0' primary key,
    available_status char(3) default '是' check (available_status in ('否', '是')),
    current_capacity varchar2(4) default '0' not null,
    curr_charge_times int default 0 not null,
    manufacturing_date timestamp default systimestamp not null,
    battery_type_id varchar2(50) default 'tp0.00.0' not null,
    constraint fk_battery_type foreign key (battery_type_id)
        references c###car.battery_type (battery_type_id) on delete cascade
);

create table c###car.vehicle_param (
    vehicle_model varchar2(50) default 'fu_wl_lp1_num0.0.0' primary key,
    transmission varchar2(50) default 'gt0.00' not null,
    service_term timestamp default systimestamp not null,
    manufacturer varchar2(50) default 'nio inc.' not null,
    snip blob,
    max_speed number
);

create table c###car.vehicle_owner (
    owner_id varchar2(50) default '000000000' primary key,
    username varchar2(50) default '鏉庡洓' not null,
    nickname varchar2(50) default 'user_0.00' not null,
    password varchar2(50) default '123456' not null,
    profile_photo blob,
    create_time timestamp default systimestamp not null,
    phone_number varchar2(50) default '+86',
    email varchar2(50) default 'wl@car.com' not null,
    gender char(3) default '男' check (gender in ('男', '女')),
    birthday timestamp,
    address varchar2(255)
);

create table c###car.employee (
    employee_id varchar2(50) default '10000000' primary key,
    username varchar2(50) default 'ep_0.00' not null,
    email varchar2(50) default 'xxxx@xxx.xxx' not null,
    password varchar2(50) default '123456' not null,
    profile_photo blob,
    create_time timestamp default systimestamp not null,
    phone_number varchar2(50) default '+86',
    identity_number varchar2(50) default '1xxxxxxxxxxxxxxxxx' not null,
    name varchar2(50) default '寮犱笁' not null,
    gender char(3) default '男' check (gender in ('男', '女')),
    positions varchar2(50) default '_',
    salary number
);

create table c###car.news (
    announcement_id varchar2(50) default 'news. 0' primary key,
    publish_time timestamp default systimestamp not null,
    publish_pos varchar2(50),
    title varchar2(50) default 'money talks.' not null,
    contents varchar2(255),
    likes int default 0,
    view_count int default 0
);

create table c###car.battery_switch_station (
    battery_id varchar2(50) default 'bt_tl1_num0.0.0' primary key,
    station_id varchar2(50) default 'st0' not null,
    constraint fk_battery_battery_switch_station foreign key (battery_id)
        references c###car.battery (battery_id) on delete cascade,
    constraint fk_battery_switch_station_switch_station foreign key (station_id)
        references c###car.switch_station (station_id) on delete cascade 
);

create table c###car.kpi (
    kpi_id varchar2(50) default 'pf. no. 0__0' primary key,
    employee_id varchar2(50)  default 'no. 0' not null,
    total_performance int default 0 not null,
    service_frequency int default 0,
    score int,
    constraint fk_kpi_employee foreign key (employee_id)
        references c###car.employee (employee_id) on delete cascade 
);
create table c###car.vehicle (
    vehicle_id varchar2(50) default 'vc. lc.0' primary key,
    vehicle_model varchar2(50) default 'fu_wl_lp1_num0.0.0' not null,
    plate_number varchar2(50) default 'x0xxxxx' not null,
    owner_id varchar2(50) default 'no. 0' not null,
    purchase_date timestamp default systimestamp not null,
    battery_id varchar2(50) default null,
    constraint fk_vehicle_owner foreign key (owner_id)
        references c###car.vehicle_owner (owner_id) on delete cascade ,
    constraint fk_vehicle_vehicle_param foreign key (vehicle_model)
        references c###car.vehicle_param (vehicle_model) on delete cascade ,
    constraint fk_vehicle_battery foreign key (battery_id)
        references c###car.battery (battery_id) on delete cascade 
);
create table c###car.maintenance_item (
    maintenance_item_id varchar2(50) default 'mti_ep_0x548151' primary key,
    vehicle_id varchar2(50) default 'vc. lc.0' not null,
    title varchar2(50) default '缁翠慨宸ヤ綔1' not null,
    maintenance_location varchar2(50) unique,
    remarks varchar2(255),
    service_time timestamp default systimestamp not null,
    order_submission_time timestamp,
    order_status char(3) default '是' check (order_status in ('否', '是')),
    evaluations varchar2(255),
    constraint fk_maintenance_item_vehicle foreign key (vehicle_id)
        references c###car.vehicle (vehicle_id) on delete cascade 
);

create table c###car.employee_switch_station (
    employee_id varchar2(50) default 'no. 0' primary key,
    station_id varchar2(50) default 'st0' not null,
    constraint fk_employee_switch_station_employee foreign key (employee_id)
        references c###car.employee (employee_id) on delete cascade ,
    constraint fk_employee_switch_station_switch_station foreign key (station_id)
        references c###car.switch_station (station_id) on delete cascade 
);
create table c###car.maintenance_item_employee (
    maintenance_item_id varchar2(50) default 'mti_ep_0x548151' primary key,
    employee_id varchar2(50) default 'no. 0' not null,
    constraint fk_maintenance_item_employee_employee foreign key (employee_id)
        references c###car.employee (employee_id) on delete cascade ,
    constraint fk_maintenance_item_employee_maintenance_item foreign key (maintenance_item_id)
        references c###car.maintenance_item (maintenance_item_id) on delete cascade 
);
create table c###car.switch_request_employee (
    switch_request_id varchar2(50) default 'sr_0x_1207xw1' primary key,
    employee_id varchar2(50) default 'no. 0' not null,
    constraint fk_switch_request_employee_employee foreign key (employee_id)
        references c###car.employee (employee_id) on delete cascade 
);

create table c###car.switch_request (
    switch_request_id varchar2(50) default 'sr_0x_1207xw1' primary key,
    vehicle_id varchar2(50) default 'vc. lc.0' not null,
    switch_type char(6) default '到店' check (switch_type in ('到店', '上门')),
    request_time timestamp default systimestamp not null,
    longtitude number default 90.00 not null unique,
    latitude number default 90.00 not null unique,
    position varchar2(50),
    remarks varchar2(255),
    constraint fk_switch_request_vehicle foreign key (vehicle_id)
        references c###car.vehicle (vehicle_id) on delete cascade ,
    constraint fk_switch_request_switch_request_employee foreign key (switch_request_id)
        references c###car.switch_request_employee (switch_request_id) on delete cascade 
);
create table c###car.switch_log (
    switch_service_id varchar2(50) default 'sw_0x_017' primary key,
    vehicle_id varchar2(50) default 'vc. lc.0' not null,
    switch_time timestamp default systimestamp not null,
    employee_id varchar2(50) default 'no. 0' not null,
    battery_id_on varchar2(50) default 'bt_tl1_num0.0.1' not null,
    battery_id_off varchar2(50) default 'bt_tl1_num0.0.0' not null,
    evaluations varchar2(255),
    longtitude number default 90.00 not null unique,
    latitude number default 90.00 not null unique,
    position varchar2(50),
    constraint fk_switch_log_vehicle foreign key (vehicle_id)
        references c###car.vehicle (vehicle_id) on delete cascade ,
    constraint fk_switch_log_employee foreign key (employee_id)
        references c###car.employee (employee_id) on delete cascade ,
    constraint fk_switch_log_battery_in foreign key (battery_id_on)
        references c###car.battery (battery_id) on delete cascade ,
    constraint fk_switch_log_battery_out foreign key (battery_id_off)
        references c###car.battery (battery_id) on delete cascade ,
    constraint fk_switch_log_switch_request_lon foreign key (longtitude)
        references c###car.switch_request (longtitude) on delete cascade ,
    constraint fk_switch_log_switch_request_lat foreign key (latitude)
        references c###car.switch_request (latitude) on delete cascade
);
create table c###car.administrator (
    admin_id varchar2(50) default '2000000' primary key,
    email varchar2(50) default 'xxxx@xxxx.xxx' not null,
    password varchar2(50) default '123456' not null,
);
create trigger c###car.trg_cascade_update_battery
after update of battery_type_id on c###car.battery_type
for each row
begin
    update c###car.battery
    set battery_type_id = :new.battery_type_id
    where battery_type_id = :old.battery_type_id;
end;
/
create trigger c###car.trg_bat_bat_replacing_sites_battery
after update of battery_id on c###car.battery
for each row
begin
    if updating('battery_id') then
        update c###car.battery_switch_station
        set battery_id = :new.battery_id
        where battery_id = :old.battery_id;
    end if;
end;
/
create trigger c###car.trg_bat_bat_replacing_sites_site
after update of station_id on c###car.switch_station
for each row
begin
    if updating('station_id') then
        update c###car.battery_switch_station
        set station_id = :new.station_id
        where station_id = :old.station_id;
    end if;
end;
/
create trigger c###car.trg_kpi_employee
after update of employee_id on c###car.employee
for each row
begin
    if updating('employee_id') then
        update c###car.kpi
        set employee_id = :new.employee_id
        where employee_id = :old.employee_id;
    end if;
end;
/
create trigger c###car.trg_vehicle_owner
after update of owner_id on c###car.vehicle_owner
for each row
begin
    if updating('owner_id') then
        update c###car.vehicle
        set owner_id = :new.owner_id
        where owner_id = :old.owner_id;
    end if;
end;
/
create trigger c###car.trg_vehicle_vehicle_param
after update of vehicle_model on c###car.vehicle_param
for each row
begin
    if updating('vehicle_model') then
        update c###car.vehicle
        set vehicle_model = :new.vehicle_model
        where vehicle_model = :old.vehicle_model;
    end if;
end;
/
create trigger c###car.trg_vehicle_battery
after update of battery_id on c###car.battery
for each row
begin
    if updating('battery_id') then
        update c###car.vehicle
        set battery_id = :new.battery_id
        where battery_id = :old.battery_id;
    end if;
end;
/
create trigger c###car.trg_maintenance_item_vehicle
after update of vehicle_id on c###car.vehicle
for each row
begin
    if updating('vehicle_id') then
        update c###car.maintenance_item
        set vehicle_id = :new.vehicle_id
        where vehicle_id = :old.vehicle_id;
    end if;
end;
/
create trigger c###car.trg_employee_switch_station_employee
after update of employee_id on c###car.employee
for each row
begin
    if updating('employee_id') then
        update c###car.employee_switch_station
        set employee_id = :new.employee_id
        where employee_id = :old.employee_id;
    end if;
end;
/
create trigger c###car.trg_employee_switch_station_site
after update of station_id on c###car.switch_station
for each row
begin
    if updating('station_id') then
        update c###car.employee_switch_station
        set station_id = :new.station_id
        where station_id = :old.station_id;
    end if;
end;
/
create trigger c###car.trg_maintenance_item_employee_employee
after update of employee_id on c###car.employee
for each row
begin
    if updating('employee_id') then
        update c###car.maintenance_item_employee
        set employee_id = :new.employee_id
        where employee_id = :old.employee_id;
    end if;
end;
/
create trigger c###car.trg_maintenance_item_employee_maintenance_item
after update of maintenance_item_id on c###car.maintenance_item
for each row
begin
    if updating('maintenance_item_id') then
        update c###car.maintenance_item_employee
        set maintenance_item_id = :new.maintenance_item_id
        where maintenance_item_id = :old.maintenance_item_id;
    end if;
end;
/
create trigger c###car.trg_switch_request_employee_employee
after update of employee_id on c###car.employee
for each row
begin
    if updating('employee_id') then
        update c###car.switch_request_employee
        set employee_id = :new.employee_id
        where employee_id = :old.employee_id;
    end if;
end;
/
create trigger c###car.trg_switch_request_vehicle
after update of vehicle_id on c###car.vehicle
for each row
begin
    if updating('vehicle_id') then
        update c###car.switch_request
        set vehicle_id = :new.vehicle_id
        where vehicle_id = :old.vehicle_id;
    end if;
end;
/
create trigger c###car.trg_switch_request_acceptance
after update of switch_request_id on c###car.switch_request_employee
for each row
begin
    if updating('switch_request_id') then
        update c###car.switch_request
        set switch_request_id = :new.switch_request_id
        where switch_request_id = :old.switch_request_id;
    end if;
end;
/
create trigger c###car.trg_switch_log_vehicle
after update of vehicle_id on c###car.vehicle
for each row
begin
    if updating('vehicle_id') then
        update c###car.switch_log
        set vehicle_id = :new.vehicle_id
        where vehicle_id = :old.vehicle_id;
    end if;
end;
/
create trigger c###car.trg_switch_log_employee
after update of employee_id on c###car.employee
for each row
begin
    if updating('employee_id') then
        update c###car.switch_log
        set employee_id = :new.employee_id
        where employee_id = :old.employee_id;
    end if;
end;
/
create trigger c###car.trg_switch_log_battery_in
after update of battery_id on c###car.battery
for each row
begin
    if updating('battery_id') then
        update c###car.switch_log
        set battery_id_on = :new.battery_id
        where battery_id_on = :old.battery_id;
    end if;
end;
/
create trigger c###car.trg_switch_log_battery_out
after update of battery_id on c###car.battery
for each row
begin
    if updating('battery_id') then
        update c###car.switch_log
        set battery_id_off = :new.battery_id
        where battery_id_off = :old.battery_id;
    end if;
end;
/
create trigger c###car.trg_switch_log_switch_request_lon
after update of longtitude on c###car.switch_request
for each row
begin
    if updating('longtitude') then
        update c###car.switch_log
        set longtitude = :new.longtitude
        where longtitude = :old.longtitude;
    end if;
end;
/
create trigger c###car.trg_switch_log_switch_request_lat
after update of latitude on c###car.switch_request
for each row
begin
    if updating('latitude') then
        update c###car.switch_log
        set latitude = :new.latitude
        where latitude = :old.latitude;
    end if;
end;
/
