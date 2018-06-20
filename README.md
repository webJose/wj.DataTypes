# wj.DataTypes

## English

Small compilation of custom datatypes to represent various common values.

### TimeStamp

*Struct* data type whose internal value is an 8-byte value.  This data type's most common use is in database scenarios where the fetched data containes a *timestamp* value meant to prevent accidental overwriting modified data between a data read and its corresponding *UPDATE* operation.

### Money

*Struct* data type meant to represent a monetary value.  Its internal value is comprised of a `decimal` value and a `string` value; the latter contains the 3-letter ISO currency code.  Its string representation defaults to the string representation of the native region for the currency, save for the decimal and thousand separators.  Decimal and thousand separators are overridden with the ones set up in the thread's current culture.

## Español

### TimeStamp

Tipo de datos *struct* cuyo valor interno es un valor de 8 bytes.  El uso más común de este tipo de dato es en escenarios de base de datos donde la información recibida contiene un valor *timestamp* cuyo propósito es prevenir la sobreescritura accidental de datos modificadoes entre la lectura de los datos y su operación *UPDATE* correspondiente.

### Money

Tipo de datos *struct* que representa un valor monetario.  Su valor interno está compuesto de un valor `decimal` y un valor `string`; el último contiene el código ISO de 3 letras de la moneda.  Su representación textual es la representación textual de la región nativa para la moneda, excepto por los separadores de decimales y miles.  Estos separadores son invalidados con los que están definidos en la cultura para el hilo de ejecución.
