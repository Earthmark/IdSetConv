using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace IdSetConv
{
  public static class Set3Loader
  {
    public static IEnumerable<(Variable, Variable, Variable)> Parse(string str)
    {
      var stream = new AntlrInputStream(str);
      var lexer = new Set3Lexer(stream);
      var tokens = new CommonTokenStream(lexer);
      var parser = new Set3Parser(tokens);
      var visitor = new Visitor();
      var expr = parser.expr();
      return visitor.VisitExpr(expr);
    }

    private class Visitor : Set3BaseVisitor<IEnumerable<(Variable, Variable, Variable)>>
    {
      public override IEnumerable<(Variable, Variable, Variable)> VisitExpr(Set3Parser.ExprContext context)
      {
        return context.trip().SelectMany(VisitTrip);
      }

      public override IEnumerable<(Variable, Variable, Variable)> VisitTrip(Set3Parser.TripContext context)
      {
        yield return (VisitSymbol(context.symb(0)), VisitSymbol(context.symb(1)), VisitSymbol(context.symb(2)));
      }

      private Variable VisitSymbol(Set3Parser.SymbContext context)
      {
        return new Variable(context.Negate() == null, context.Token().GetText());
      }
    }
  }
}
