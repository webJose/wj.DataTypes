# wj.DataTypes

## English

Small compilation of custom datatypes to represent various common values.

### RowVersion

*Struct* data type whose internal value is an 8-byte value.  This data type's most common use is in database scenarios where the fetched data contains a *"timestamp"* value of some sort meant to prevent accidental overwriting of modified data between a data read and its corresponding *UPDATE* operation.

In Microsoft SQL Server, this is a `rowversion` column (formerly known as `timestamp`); in Oracle this would be the 6-byte value `ORA_ROWSCN` column provided by Oracle (use `rowdependencies`); in MySQL it would be a `timestamp` column (4 to 7 bytes) with automatic update.

### Money

*Struct* data type meant to represent a monetary value.  Its internal value is comprised of a `decimal` value and a `string` value; the latter contains the 3-letter ISO currency code.  Its string representation defaults to the string representation of the native region for the currency, save for the decimal and group separators.  Decimal and group separators are overridden with the ones set up in the thread's current culture.

## Español

### RowVersion

Tipo de datos *struct* cuyo valor interno es un valor de 8 bytes.  El uso más común de este tipo de dato es en escenarios de base de datos donde la información recibida contiene un valor *timestamp* cuyo propósito es prevenir la sobreescritura accidental de datos modificadoes entre la lectura de los datos y su operación *UPDATE* correspondiente.

En Microsoft SQL Server, esto sería una columna tipo `rowversion` (antes conocida como `timestamp`); en Oracle esto sería el valor de 6 bytes de la columna `ORA_ROWSCN` provista por Oracle (utilice `rowdependencies`); en MySQL sería una columna tipo `timestamp` (4 a 7 bytes) con actualización automática.

### Money

Tipo de datos *struct* que representa un valor monetario.  Su valor interno está compuesto de un valor `decimal` y un valor `string`; el último contiene el código ISO de 3 letras de la moneda.  Su representación textual es la representación textual de la región nativa para la moneda, excepto por los separadores de decimales y de grupos.  Estos separadores son invalidados con los que están definidos en la cultura para el hilo de ejecución.
