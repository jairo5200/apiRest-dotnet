-- Creamos la función para Listar los países
CREATE OR REPLACE FUNCTION obtener_paises()
RETURNS TABLE(id INT, nombre VARCHAR(100)) AS $$
BEGIN
    RETURN QUERY SELECT p.id, p.nombre FROM Paises p;
END;
$$ LANGUAGE plpgsql;

--Creamos la función para obtener un país por id
CREATE OR REPLACE FUNCTION obtener_pais_por_id(p_id INT)
RETURNS TABLE(id INT, nombre VARCHAR(100)) AS $$
BEGIN
    RETURN QUERY SELECT p.id, p.nombre FROM Paises p WHERE p.id = p_id;
END;
$$ LANGUAGE plpgsql;

--Creamos el procedimiento para guardar/actualizar un país
CREATE OR REPLACE PROCEDURE guardar_pais(p_id INT, p_nombre VARCHAR(100))
LANGUAGE plpgsql AS $$
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        INSERT INTO Paises(nombre) VALUES (p_nombre);
    ELSE
        UPDATE Paises p SET nombre = p_nombre WHERE p.id = p_id;
    END IF;
END;
$$;

--Creamos el procedimiento para eliminar País
CREATE OR REPLACE PROCEDURE eliminar_pais(p_id INT)
LANGUAGE plpgsql AS $$
BEGIN
    DELETE FROM Paises p WHERE p.id = p_id;
END;
$$;

--Creamos el procedimiento que verifica si el País tiene Departamentos
CREATE OR REPLACE PROCEDURE eliminar_pais(p_id INT)
LANGUAGE plpgsql AS $$
BEGIN
    -- Verifica si el país tiene departamentos asociados
    IF EXISTS (SELECT 1 FROM Departamentos WHERE id_pais = p_id) THEN
        -- Si tiene departamentos asociados, no lo elimina
        RAISE EXCEPTION 'No se puede eliminar el país porque tiene departamentos asociados.';
    ELSE
        -- Si no tiene departamentos asociados, lo elimina
        DELETE FROM Paises p WHERE p.id = p_id;
    END IF;
END;
$$;

--Creamos la función para obtener todos los departamentos con su país
CREATE OR REPLACE FUNCTION obtener_departamentos()
RETURNS TABLE(id INT, nombre VARCHAR(100), id_pais INT, pais_nombre VARCHAR(100)) AS $$
BEGIN
    RETURN QUERY SELECT d.id, d.nombre, d.id_pais, p.nombre AS pais_nombre
    FROM Departamentos d
    JOIN Paises p ON d.id_pais = p.id;
END;
$$ LANGUAGE plpgsql;

--Creamos la función para obtener un Departamento por id
CREATE OR REPLACE FUNCTION obtener_departamento_por_id(elId INT)
RETURNS TABLE(id INT, nombre VARCHAR(100), id_pais INT) AS $$
BEGIN
    RETURN QUERY SELECT d.id, d.nombre, d.id_pais
    FROM Departamentos d
    WHERE d.id = elId;
END;
$$ LANGUAGE plpgsql;

--Creamos el procedimiento para guardar/actualizar el departamento
CREATE OR REPLACE PROCEDURE guardar_departamento(
    elId INT,
    elNombre VARCHAR(100),
    elId_pais INT
) AS $$
BEGIN
    IF elId IS NULL OR elId = 0 THEN
        INSERT INTO Departamentos (nombre, id_pais) VALUES (elNombre, elId_pais);
    ELSE
        UPDATE Departamentos SET nombre = elNombre, id_pais = elId_pais WHERE id = elId;
    END IF;
END;
$$ LANGUAGE plpgsql;

--Creamos el procedimiento para eliminar el departamento
CREATE OR REPLACE PROCEDURE eliminar_departamento(IN elId INT)
AS $$
BEGIN
    -- Verificar si el departamento tiene municipios asociados
    IF EXISTS (SELECT 1 FROM Municipios WHERE id_departamento = elId) THEN
        -- Si tiene municipios, lanzar una excepción
        RAISE EXCEPTION 'No se puede eliminar el departamento porque tiene municipios asociados.';
    ELSE
        -- Si no tiene municipios, eliminar el departamento
        DELETE FROM Departamentos WHERE id = elId;
    END IF;
END;
$$ LANGUAGE plpgsql;

--Creamos la función para obtener los municipios
CREATE OR REPLACE FUNCTION obtener_municipios()
RETURNS TABLE(id INT, nombre VARCHAR(100), id_Departamento INT, departamento_nombre VARCHAR(100)) AS $$
BEGIN
    RETURN QUERY SELECT m.id, m.nombre, m.id_departamento, d.nombre AS departamento_nombre
    FROM municipios m
    JOIN departamentos d ON m.id_departamento = d.id;
END;
$$ LANGUAGE plpgsql;

--Creamos la función para obtener el municipio por id
CREATE OR REPLACE FUNCTION obtener_municipio_detalle(elId INT)
RETURNS TABLE (
    id INT,
    nombre VARCHAR(100),
    id_departamento INT,
    departamentos_nombre VARCHAR(100)[]
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        m.id,
        m.nombre,
        m.id_departamento,
        ARRAY(SELECT d.nombre FROM Departamentos d) AS departamentos_nombre
    FROM Municipios m
    WHERE m.id = elId;
END;
$$ LANGUAGE plpgsql;

--Creamos el procedimiento para guardar/actualizar el municipio
CREATE OR REPLACE PROCEDURE guardar_municipio(
    elId INT,
    elNombre VARCHAR(100),
    elIdDepartamento INT
) AS $$
BEGIN
    IF elId = 0 THEN
        INSERT INTO Municipios (nombre, id_departamento) VALUES (elNombre, elIdDepartamento);
    ELSE
        UPDATE Municipios SET nombre = elNombre, id_departamento = elIdDepartamento WHERE id = elId;
    END IF;
END;
$$ LANGUAGE plpgsql;

--Creamos el procedimiento para eliminar el municipio
CREATE OR REPLACE PROCEDURE eliminar_municipio(
    elId INT
) AS $$
BEGIN
    -- Verifica si el municipio tiene personas asociadas
    IF EXISTS (SELECT 1 FROM Personas WHERE id_municipio = elId) THEN
        RAISE EXCEPTION 'No se puede eliminar el municipio porque tiene personas asociadas.';
    ELSE
        DELETE FROM Municipios WHERE id = elId;
    END IF;
END;
$$ LANGUAGE plpgsql;

--Creamos la función para obtener las Personas
CREATE OR REPLACE FUNCTION obtener_personas()
RETURNS TABLE(id INT, nombre VARCHAR(100),telefono BIGINT,direccion VARCHAR(100), id_municipio INT, municipio_nombre VARCHAR(100)) AS $$
BEGIN
    RETURN QUERY SELECT p.id, p.nombre,p.telefono,p.direccion, p.id_municipio, m.nombre AS municipio_nombre
    FROM personas p
    JOIN municipios m ON p.id_municipio = m.id;
END;
$$ LANGUAGE plpgsql;

--Creamos la función para obtener una persona por su id
CREATE OR REPLACE FUNCTION obtener_persona_detalle(
    elId INT
)
RETURNS TABLE (
    id INT,
    nombre VARCHAR(100),
    telefono BIGINT,
    direccion VARCHAR(100),
    id_municipio INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        p.id,
        p.nombre,
        p.telefono,
        p.direccion,
        p.id_municipio
    FROM Personas p
    WHERE p.id = elId;
END;
$$ LANGUAGE plpgsql;

--Creamos el procedimiento para guarder/actualizar Persona
CREATE OR REPLACE PROCEDURE guardar_persona(
    elId INT,
    elNombre VARCHAR(100),
    elTelefono BIGINT,
    laDireccion VARCHAR(100),
    elIdMunicipio INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF elId = 0 THEN
        INSERT INTO Personas (nombre, telefono, direccion, id_municipio)
        VALUES (elNombre, elTelefono, laDireccion, elIdMunicipio);
    ELSE
        UPDATE Personas
        SET nombre = elNombre, telefono = elTelefono, direccion = laDireccion, id_municipio = elIdMunicipio
        WHERE id = elId;
    END IF;
END;
$$;

--Creamos el procedimiento para eliminar Persona
CREATE OR REPLACE PROCEDURE eliminar_persona(elId INT)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM Personas WHERE id = elId;
END;
$$;
