create table account (
   account_login        VARCHAR(20)          not null,
   account_password     VARCHAR(255)         not null,
   constraint PK_ACCOUNT primary key (account_login)
);

create unique index account_PK on account (
account_login
);

create table match (
   match_id             SERIAL not null,
   match_start_date     timestamp                 not null,
   match_end_date       timestamp                 null,
   constraint PK_MATCH primary key (match_id)
);

create unique index match_PK on match (
match_id
);

create table step (
   step_id              SERIAL not null,
   match_id             INT4                 not null,
   account_login        VARCHAR(20)          not null,
   attack_place         VARCHAR(5)           not null,
   attack_result        VARCHAR(20)          not null,
   first_player_ships   INT4                 not null,
   second_player_ships  INT4                 not null,
   constraint PK_STEP primary key (step_id)
);

create unique index step_PK on step (
step_id
);

create  index have_FK on step (
match_id
);

create  index make_step_FK on step (
match_id,
account_login
);

create table take_part (
   match_id             INT4                 not null,
   account_login        VARCHAR(20)          not null,
   is_winner            BOOL                 not null,
   constraint PK_TAKE_PART primary key (match_id, account_login)
);

create unique index take_part_PK on take_part (
match_id,
account_login
);

create  index take_part2_FK on take_part (
account_login
);

create  index take_part_FK on take_part (
match_id
);

alter table step
   add constraint FK_STEP_HAVE_MATCH foreign key (match_id)
      references match (match_id)
      on delete restrict on update restrict;

alter table step
   add constraint FK_STEP_MAKE_STEP_TAKE_PAR foreign key (match_id, account_login)
      references take_part (match_id, account_login)
      on delete restrict on update restrict;

alter table take_part
   add constraint FK_TAKE_PAR_TAKE_PART_MATCH foreign key (match_id)
      references match (match_id)
      on delete restrict on update restrict;

alter table take_part
   add constraint FK_TAKE_PAR_TAKE_PART_ACCOUNT foreign key (account_login)
      references account (account_login)
      on delete restrict on update restrict;

