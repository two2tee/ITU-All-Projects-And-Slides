drop table if exists Part;
create table Part (
  PartId int primary key not null,
  PartName char(50) not null,
  IsBasic int not null
);


insert into Part (PartId, PartName, IsBasic) values (0, 'Part 0', 1);
insert into Part (PartId, PartName, IsBasic) values (1, 'Part 1', 1);
insert into Part (PartId, PartName, IsBasic) values (2, 'Part 2', 0);
insert into Part (PartId, PartName, IsBasic) values (3, 'Part 3', 0);

drop table if exists PartsList;

create table PartsList (
  PartsListId int not null,
  PartId int not null,
  Quantity int not null,
  primary key (PartsListId, PartId)
);

insert into PartsList (PartsListId, PartId, Quantity) values (2, 0, 5);
insert into PartsList (PartsListId, PartId, Quantity) values (2, 1, 4);
insert into PartsList (PartsListId, PartId, Quantity) values (3, 1, 3);
insert into PartsList (PartsListId, PartId, Quantity) values (3, 2, 4);
