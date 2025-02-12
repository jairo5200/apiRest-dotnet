--Creamos la base de datos
CREATE DATABASE pruebacoinkdb
    WITH
    OWNER = admin
    ENCODING = 'UTF8'
    LOCALE_PROVIDER = 'libc'
    CONNECTION LIMIT = -1

--Creamos la tabla de países
CREATE TABLE PAISES(id serial primary key,
nombre varchar(100));

--Creamos la tabla departamentos
CREATE TABLE DEPARTAMENTOS(id serial primary key,
nombre varchar(100),
id_pais int references paises(id));

--Creamos la tabla municipios
CREATE TABLE MUNICIPIOS(id serial primary key,
nombre varchar(100),
id_departamento int references departamentos(id));

--Creamos la tabla personas
CREATE TABLE PERSONAS(id serial primary key,
nombre varchar(100),
telefono bigint,
direccion varchar(100),
id_municipio int references municipios(id));

creamos los modelos con la base de datos:
Scaffold-DbContext "Server=localhost;Port=5432;User Id=admin;Password=admin;Database=pruebacoinkdb;" Npgsql.EntityFrameworkCore.PostgreSQL  -OutPutDir  Models
