grammar Set3;

expr: trip ('&' trip)*;

trip: symb '|' symb '|' symb;

symb: Negate? Token;

Negate: '!';
Token: [A-Za-z0-9]+;

WS: [ \t\n\r] -> channel(HIDDEN);
