// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.Common;
using PdfToSvg.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Parser = PdfToSvg.Fonts.CharStrings.CharStringParser;

namespace PdfToSvg.Fonts.CharStrings
{
    internal static class CharStringOperators
    {
        private static readonly Dictionary<CharStringOpCode, CharStringOperator> operators = new()
        {
            // Path construction operators
            { CharStringOpCode.RMoveTo, new CharStringOperator(Op_RMoveTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HMoveTo, new CharStringOperator(Op_HMoveTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.VMoveTo, new CharStringOperator(Op_VMoveTo, CharStringOperatorOptions.ClearStack) },

            { CharStringOpCode.RLineTo, new CharStringOperator(Op_RLineTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HLineTo, new CharStringOperator(Op_HLineTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.VLineTo, new CharStringOperator(Op_VLineTo, CharStringOperatorOptions.ClearStack) },

            { CharStringOpCode.RRCurveTo, new CharStringOperator(Op_RRCurveTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HHCurveTo, new CharStringOperator(Op_HHCurveTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HVCurveTo, new CharStringOperator(Op_HVCurveTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.RCurveLine, new CharStringOperator(Op_RCurveLine, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.RLineCurve, new CharStringOperator(Op_RLineCurve, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.VHCurveTo, new CharStringOperator(Op_VHCurveTo, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.VVCurveTo, new CharStringOperator(Op_VVCurveTo, CharStringOperatorOptions.ClearStack) },

            { CharStringOpCode.Flex, new CharStringOperator(Op_Flex, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HFlex, new CharStringOperator(Op_HFlex, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HFlex1, new CharStringOperator(Op_HFlex1, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.Flex1, new CharStringOperator(Op_Flex1, CharStringOperatorOptions.ClearStack) },

            // Operator for finishing a path
            { CharStringOpCode.EndChar, new CharStringOperator(Op_EndChar, CharStringOperatorOptions.ClearStack) },

            // Hint operators
            { CharStringOpCode.HStem, new CharStringOperator(Op_HStem, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.VStem, new CharStringOperator(Op_VStem, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HStemHm, new CharStringOperator(Op_HStemHm, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.VStemHm, new CharStringOperator(Op_VStemHm, CharStringOperatorOptions.ClearStack) },

            { CharStringOpCode.HintMask, new CharStringOperator(Op_HintMask, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.CntrMask, new CharStringOperator(Op_CntrMask, CharStringOperatorOptions.ClearStack) },

            // Arithmetic operators
            { CharStringOpCode.Abs, new CharStringOperator(Op_Abs) },
            { CharStringOpCode.Add, new CharStringOperator(Op_Add) },
            { CharStringOpCode.Sub, new CharStringOperator(Op_Sub) },
            { CharStringOpCode.Div, new CharStringOperator(Op_Div) },
            { CharStringOpCode.Neg, new CharStringOperator(Op_Neg) },
            { CharStringOpCode.Random, new CharStringOperator(Op_Random) },
            { CharStringOpCode.Mul, new CharStringOperator(Op_Mul) },
            { CharStringOpCode.Sqrt, new CharStringOperator(Op_Sqrt) },
            { CharStringOpCode.Drop, new CharStringOperator(Op_Drop) },
            { CharStringOpCode.Exch, new CharStringOperator(Op_Exch) },
            { CharStringOpCode.Index, new CharStringOperator(Op_Index) },
            { CharStringOpCode.Roll, new CharStringOperator(Op_Roll) },
            { CharStringOpCode.Dup, new CharStringOperator(Op_Dup) },

            // Storage operators
            { CharStringOpCode.Put, new CharStringOperator(Op_Put) },
            { CharStringOpCode.Get, new CharStringOperator(Op_Get) },

            // Conditional operators
            { CharStringOpCode.And, new CharStringOperator(Op_And) },
            { CharStringOpCode.Or, new CharStringOperator(Op_Or) },
            { CharStringOpCode.Not, new CharStringOperator(Op_Not) },
            { CharStringOpCode.Eq, new CharStringOperator(Op_Eq) },
            { CharStringOpCode.IfElse, new CharStringOperator(Op_IfElse) },

            // Subroutine operators
            { CharStringOpCode.CallSubr, new CharStringOperator(Op_CallSubr) },
            { CharStringOpCode.CallGSubr, new CharStringOperator(Op_CallGSubr) },
            { CharStringOpCode.Return, new CharStringOperator(Op_Return) },

            // Deprecated operators
            { CharStringOpCode.DotSection, new CharStringOperator(Op_DotSection) },

            // Type 1 operators
            { CharStringOpCode.VStem3, new CharStringOperator(Op_VStem3, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.HStem3, new CharStringOperator(Op_HStem3, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.Pop, new CharStringOperator(Op_Pop) },
            { CharStringOpCode.Hsbw, new CharStringOperator(Op_Hsbw, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.Seac, new CharStringOperator(Op_Seac, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.Sbw, new CharStringOperator(Op_Sbw, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.CallOtherSubr, new CharStringOperator(Op_CallOtherSubr, CharStringOperatorOptions.ClearStack) },
            { CharStringOpCode.ClosePath, new CharStringOperator(Op_ClosePath) },
            { CharStringOpCode.SetCurrentPoint, new CharStringOperator(Op_SetCurrentPoint) },
        };

        public static bool TryGetOperator(CharStringOpCode code, [MaybeNullWhen(false)] out CharStringOperator result)
        {
            return operators.TryGetValue(code, out result);
        }

        #region Path construction operators

        private static void Op_RMoveTo(Parser parser)
        {
            if (parser.FlexPoints == null)
            {
                parser.AppendContent(CharStringOpCode.RMoveTo, last: 2);
                parser.Stack.Pop(out double dx1, out double dy1);
                parser.Path.RMoveTo(dx1, dy1);
            }
            else
            {
                parser.Stack.Pop(out double dx1, out double dy1);
                parser.FlexPoints.Add(new Point(dx1, dy1));
            }
        }

        private static void Op_HMoveTo(Parser parser)
        {
            if (parser.FlexPoints == null)
            {
                parser.AppendContent(CharStringOpCode.HMoveTo, last: 1);
                parser.Stack.Pop(out double dx1);
                parser.Path.RMoveTo(dx1, 0);
            }
            else
            {
                parser.Stack.Pop(out double dx1);
                parser.FlexPoints.Add(new Point(dx1, 0));
            }
        }

        private static void Op_VMoveTo(Parser parser)
        {
            if (parser.FlexPoints == null)
            {
                parser.AppendContent(CharStringOpCode.VMoveTo, last: 1);
                parser.Stack.Pop(out double dy1);
                parser.Path.RMoveTo(0, dy1);
            }
            else
            {
                parser.Stack.Pop(out double dy1);
                parser.FlexPoints.Add(new Point(0, dy1));
            }
        }

        private static void Op_RLineTo(Parser parser)
        {
            var startAt = parser.Stack.Count % 2;

            for (var i = startAt; i < parser.Stack.Count; i += 2)
            {
                parser.Path.RLineTo(parser.Stack[i], parser.Stack[i + 1]);
            }

            parser.AppendContent(CharStringOpCode.RLineTo, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        private static void AlternatingLineTo(Parser parser, bool startHorizontally)
        {
            var horizontal = startHorizontally;

            parser.AppendContent(
                startHorizontally ? CharStringOpCode.HLineTo : CharStringOpCode.VLineTo,
                from: 0);

            for (var i = 0; i < parser.Stack.Count; i++)
            {
                if (horizontal)
                {
                    parser.Path.RLineTo(parser.Stack[i], 0);
                }
                else
                {
                    parser.Path.RLineTo(0, parser.Stack[i]);
                }

                horizontal = !horizontal;
            }

            parser.Stack.Clear();
        }

        private static void Op_HLineTo(Parser parser)
        {
            AlternatingLineTo(parser, startHorizontally: true);
        }

        private static void Op_VLineTo(Parser parser)
        {
            AlternatingLineTo(parser, startHorizontally: false);
        }

        private static void Op_RRCurveTo(Parser parser)
        {
            var startAt = parser.Stack.Count % 6;

            for (var i = startAt; i < parser.Stack.Count; i += 6)
            {
                parser.Path.RRCurveTo(
                    parser.Stack[i + 0],
                    parser.Stack[i + 1],
                    parser.Stack[i + 2],
                    parser.Stack[i + 3],
                    parser.Stack[i + 4],
                    parser.Stack[i + 5]
                    );
            }

            parser.AppendContent(CharStringOpCode.RRCurveTo, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_HHCurveTo(Parser parser)
        {
            var startAt = parser.Stack.Count % 4;
            int removeFrom;

            if (startAt > 0 && startAt + 3 < parser.Stack.Count)
            {
                removeFrom = startAt - 1;

                parser.Path.RRCurveTo(
                    parser.Stack[startAt + 0],
                    parser.Stack[startAt - 1],
                    parser.Stack[startAt + 1],
                    parser.Stack[startAt + 2],
                    parser.Stack[startAt + 3],
                    0);

                startAt += 4;
            }
            else
            {
                removeFrom = startAt;
            }

            for (var i = startAt; i < parser.Stack.Count; i += 4)
            {
                parser.Path.RRCurveTo(
                    parser.Stack[i + 0],
                    0,
                    parser.Stack[i + 1],
                    parser.Stack[i + 2],
                    parser.Stack[i + 3],
                    0);
            }

            parser.AppendContent(CharStringOpCode.HHCurveTo, from: removeFrom);
            parser.Stack.RemoveFrom(removeFrom);
        }

        private static void AlternatingHVCurveTo(Parser parser, bool startHorizontal)
        {
            if (parser.Stack.Count < 4)
            {
                throw new CharStringStackUnderflowException();
            }

            var startAt = parser.Stack.Count % 4;
            var endAt = parser.Stack.Count;
            var endOrthogonal = true;

            if (startAt > 0)
            {
                startAt--;
                endAt--;
                endOrthogonal = false;
            }

            parser.AppendContent(
                startHorizontal ? CharStringOpCode.HVCurveTo : CharStringOpCode.VHCurveTo,
                from: startAt);

            for (var i = startAt; i < endAt; i += 4)
            {
                var lastd = 0d;

                if (!endOrthogonal && i + 4 == endAt)
                {
                    lastd = parser.Stack[i + 4];
                }

                if (startHorizontal)
                {
                    parser.Path.RRCurveTo(
                        parser.Stack[i + 0],
                        0,
                        parser.Stack[i + 1],
                        parser.Stack[i + 2],
                        lastd,
                        parser.Stack[i + 3]);

                    startHorizontal = false;
                }
                else
                {
                    parser.Path.RRCurveTo(
                        0,
                        parser.Stack[i + 0],
                        parser.Stack[i + 1],
                        parser.Stack[i + 2],
                        parser.Stack[i + 3],
                        lastd);

                    startHorizontal = true;
                }
            }

            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_HVCurveTo(Parser parser)
        {
            AlternatingHVCurveTo(parser, startHorizontal: true);
        }

        private static void Op_RCurveLine(Parser parser)
        {
            if (parser.Stack.Count < 8)
            {
                throw new CharStringStackUnderflowException();
            }

            var startAt = (parser.Stack.Count - 2) % 6;

            for (var i = startAt; i + 2 < parser.Stack.Count; i += 6)
            {
                parser.Path.RRCurveTo(
                   parser.Stack[i + 0],
                   parser.Stack[i + 1],
                   parser.Stack[i + 2],
                   parser.Stack[i + 3],
                   parser.Stack[i + 4],
                   parser.Stack[i + 5]);
            }

            parser.Path.RLineTo(
                parser.Stack[parser.Stack.Count - 2],
                parser.Stack[parser.Stack.Count - 1]
                );

            parser.AppendContent(CharStringOpCode.RCurveLine, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_RLineCurve(Parser parser)
        {
            if (parser.Stack.Count < 8)
            {
                throw new CharStringStackUnderflowException();
            }

            var startAt = parser.Stack.Count % 2;

            for (var i = startAt; i + 6 < parser.Stack.Count; i += 2)
            {
                parser.Path.RLineTo(
                   parser.Stack[i + 0],
                   parser.Stack[i + 1]);
            }

            parser.Path.RRCurveTo(
                parser.Stack[parser.Stack.Count - 6],
                parser.Stack[parser.Stack.Count - 5],
                parser.Stack[parser.Stack.Count - 4],
                parser.Stack[parser.Stack.Count - 3],
                parser.Stack[parser.Stack.Count - 2],
                parser.Stack[parser.Stack.Count - 1]);

            parser.AppendContent(CharStringOpCode.RLineCurve, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_VHCurveTo(Parser parser)
        {
            AlternatingHVCurveTo(parser, startHorizontal: false);
        }

        private static void Op_VVCurveTo(Parser parser)
        {
            if (parser.Stack.Count < 4)
            {
                throw new CharStringStackUnderflowException();
            }

            var startAt = parser.Stack.Count % 4;
            var removeFrom = startAt;
            var dx1 = 0d;

            if (startAt > 0)
            {
                dx1 = parser.Stack[startAt - 1];
                removeFrom--;
            }

            for (var i = startAt; i < parser.Stack.Count; i += 4)
            {
                parser.Path.RRCurveTo(
                    dx1,
                    parser.Stack[i + 0],
                    parser.Stack[i + 1],
                    parser.Stack[i + 2],
                    0,
                    parser.Stack[i + 3]);

                dx1 = 0;
            }

            parser.AppendContent(CharStringOpCode.VVCurveTo, from: removeFrom);
            parser.Stack.RemoveFrom(removeFrom);
        }

        private static void Op_Flex(Parser parser)
        {
            var startAt = parser.Stack.Count - 13;
            if (startAt < 0)
            {
                throw new CharStringStackUnderflowException();
            }

            parser.Path.RRCurveTo(
                parser.Stack[startAt + 0],
                parser.Stack[startAt + 1],
                parser.Stack[startAt + 2],
                parser.Stack[startAt + 3],
                parser.Stack[startAt + 4],
                parser.Stack[startAt + 5]
                );

            parser.Path.RRCurveTo(
                parser.Stack[startAt + 6],
                parser.Stack[startAt + 7],
                parser.Stack[startAt + 8],
                parser.Stack[startAt + 9],
                parser.Stack[startAt + 10],
                parser.Stack[startAt + 11]
                );

            parser.AppendContent(CharStringOpCode.Flex, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_HFlex(Parser parser)
        {
            var startAt = parser.Stack.Count - 7;
            if (startAt < 0)
            {
                throw new CharStringStackUnderflowException();
            }

            parser.Path.RRCurveTo(
                parser.Stack[startAt + 0],
                0,
                parser.Stack[startAt + 1],
                parser.Stack[startAt + 2],
                parser.Stack[startAt + 3],
                0);

            parser.Path.RRCurveTo(
                parser.Stack[startAt + 4],
                0,
                parser.Stack[startAt + 5],
                0,
                parser.Stack[startAt + 6],
                0);

            parser.AppendContent(CharStringOpCode.HFlex, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_HFlex1(Parser parser)
        {
            var startAt = parser.Stack.Count - 9;
            if (startAt < 0)
            {
                throw new CharStringStackUnderflowException();
            }

            parser.Path.RRCurveTo(
                parser.Stack[startAt + 0],
                parser.Stack[startAt + 1],
                parser.Stack[startAt + 2],
                parser.Stack[startAt + 3],
                parser.Stack[startAt + 4],
                0);

            parser.Path.RRCurveTo(
                parser.Stack[startAt + 5],
                0,
                parser.Stack[startAt + 6],
                parser.Stack[startAt + 7],
                parser.Stack[startAt + 8],
                0);

            parser.AppendContent(CharStringOpCode.HFlex1, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_Flex1(Parser parser)
        {
            var startAt = parser.Stack.Count - 11;
            if (startAt < 0)
            {
                throw new CharStringStackUnderflowException();
            }

            var dx1 = parser.Stack[startAt + 0];
            var dy1 = parser.Stack[startAt + 1];
            var dx2 = parser.Stack[startAt + 2];
            var dy2 = parser.Stack[startAt + 3];
            var dx3 = parser.Stack[startAt + 4];
            var dy3 = parser.Stack[startAt + 5];
            var dx4 = parser.Stack[startAt + 6];
            var dy4 = parser.Stack[startAt + 7];
            var dx5 = parser.Stack[startAt + 8];
            var dy5 = parser.Stack[startAt + 9];
            var d6 = parser.Stack[startAt + 10];

            var dx = dx1 + dx2 + dx3 + dx4 + dx5;
            var dy = dy1 + dy2 + dy3 + dy4 + dy5;

            double dx6, dy6;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                dx6 = d6;
                dy6 = 0;
            }
            else
            {
                dx6 = 0;
                dy6 = d6;
            }

            parser.Path.RRCurveTo(dx1, dy1, dx2, dy2, dx3, dy3);
            parser.Path.RRCurveTo(dx4, dy4, dx5, dy5, dx6, dy6);

            parser.AppendContent(CharStringOpCode.Flex1, from: startAt);
            parser.Stack.RemoveFrom(startAt);
        }

        #endregion

        #region Operator for finishing a path

        private static void Op_EndChar(Parser parser)
        {
            if (parser.Stack.Count >= 4)
            {
                parser.CharString.Seac = new CharStringSeacInfo(
                    adx: parser.Stack[parser.Stack.Count - 4],
                    ady: parser.Stack[parser.Stack.Count - 3],
                    bchar: (int)parser.Stack[parser.Stack.Count - 2],
                    achar: (int)parser.Stack[parser.Stack.Count - 1]);

                parser.AppendContent(CharStringOpCode.EndChar, last: 4);
                parser.Stack.RemoveFrom(parser.Stack.Count - 4);
            }
            else
            {
                parser.AppendContent(CharStringOpCode.EndChar);
            }

            parser.EndChar();
        }

        #endregion

        #region Hint operators

        private static void Hint(Parser parser, bool isHorizontal, CharStringOpCode? opCode = null)
        {
            if (parser.Stack.Count < 2)
            {
                return;
            }

            // All hints use an even number of arguments
            var startAt = parser.Stack.Count % 2;

            // We could handle hints from type 1 char strings as well, but as long as we don't support turning on and
            // off hints by othersubr #3, they will not work optimally. After testing, it was decided to ignore hints
            // as it caused better and more consistent results than having all hints enabled all the time.
            if (parser.Type == CharStringType.Type2)
            {
                parser.CharString.HintCount += parser.Stack.Count / 2;

                if (startAt < parser.Stack.Count)
                {
                    var sideBearing = isHorizontal
                        ? parser.Path.LastY
                        : parser.Path.LastX;

                    parser.Stack[startAt] = parser.Stack[startAt] + sideBearing;

                    for (var i = startAt; i < parser.Stack.Count; i++)
                    {
                        parser.CharString.Hints.Add(CharStringLexeme.Operand(parser.Stack[i]));
                    }
                }

                if (opCode.HasValue)
                {
                    parser.CharString.Hints.Add(CharStringLexeme.Operator(opCode.Value));
                }
            }

            parser.Stack.RemoveFrom(startAt);
        }

        private static void Op_HStem(Parser parser)
        {
            Hint(parser, isHorizontal: true, CharStringOpCode.HStem);
        }

        private static void Op_VStem(Parser parser)
        {
            Hint(parser, isHorizontal: false, CharStringOpCode.VStem);
        }

        private static void Op_HStemHm(Parser parser)
        {
            Hint(parser, isHorizontal: true, CharStringOpCode.HStemHm);
        }

        private static void Op_VStemHm(Parser parser)
        {
            Hint(parser, isHorizontal: false, CharStringOpCode.VStemHm);
        }

        private static void Mask(Parser parser, CharStringOpCode opCode)
        {
            // vstem hint operator is optional if hstem and vstem direcly preceeds the hintmask operator.
            if (parser.CharString.Content.Count == 0)
            {
                Hint(parser, isHorizontal: false);
            }

            // Mask
            var maskBytes = MathUtils.BitsToBytes(parser.CharString.HintCount);
            if (maskBytes > 0)
            {
                parser.CharString.Content.Add(CharStringLexeme.Operator(opCode));

                for (var i = 0; i < maskBytes; i++)
                {
                    var lexeme = CharStringLexeme.Mask(parser.Lexer.ReadByte());
                    parser.CharString.Content.Add(lexeme);
                }
            }
        }

        private static void Op_HintMask(Parser parser)
        {
            Mask(parser, CharStringOpCode.HintMask);
        }

        private static void Op_CntrMask(Parser parser)
        {
            Mask(parser, CharStringOpCode.CntrMask);
        }

        #endregion

        #region Arithmetic operators

        private static void Op_Abs(Parser parser)
        {
            parser.Stack.Pop(out double num1);
            parser.Stack.Push(Math.Abs(num1));
        }

        private static void Op_Add(Parser parser)
        {
            parser.Stack.Pop(out double num1, out double num2);
            parser.Stack.Push(num1 + num2);
        }

        private static void Op_Sub(Parser parser)
        {
            parser.Stack.Pop(out double num1, out double num2);
            parser.Stack.Push(num1 - num2);
        }

        private static void Op_Div(Parser parser)
        {
            parser.Stack.Pop(out double num1, out double num2);
            parser.Stack.Push(num1 / num2);
        }

        private static void Op_Neg(Parser parser)
        {
            parser.Stack.Pop(out double num1);
            parser.Stack.Push(-num1);
        }

        private static void Op_Random(Parser parser)
        {
            // PdfToSvg.NET does not support the random operator. It will always generate 0.42.
            parser.Stack.Push(0.42d);
        }

        private static void Op_Mul(Parser parser)
        {
            parser.Stack.Pop(out double num1, out double num2);
            parser.Stack.Push(num1 * num2);
        }

        private static void Op_Sqrt(Parser parser)
        {
            parser.Stack.Pop(out double num1);
            parser.Stack.Push(Math.Sqrt(num1));
        }

        private static void Op_Drop(Parser parser)
        {
            parser.Stack.Pop(out int n);
            parser.Stack.RemoveFrom(MathUtils.Clamp(parser.Stack.Count - n, 0, parser.Stack.Count));
        }

        private static void Op_Exch(Parser parser)
        {
            if (parser.Stack.Count < 2)
            {
                throw new CharStringStackUnderflowException();
            }

            var index1 = parser.Stack.Count - 1;
            var index2 = parser.Stack.Count - 2;

            var tmp = parser.Stack[index1];
            parser.Stack[index1] = parser.Stack[index2];
            parser.Stack[index2] = tmp;
        }

        private static void Op_Index(Parser parser)
        {
            parser.Stack.Pop(out int n);

            if (n < 0)
            {
                n = 0;
            }

            var index = parser.Stack.Count - n - 1;
            if (index < 0)
            {
                throw new CharStringStackUnderflowException();
            }

            parser.Stack.Push(parser.Stack[index]);
        }

        private static void Op_Roll(Parser parser)
        {
            parser.Stack.Pop(out int n, out int j);
            parser.Stack.Roll(n, j);
        }

        private static void Op_Dup(Parser parser)
        {
            parser.Stack.Push(parser.Stack.Peek());
        }

        #endregion

        #region Storage operators

        private static void Op_Put(Parser parser)
        {
            parser.Stack.Pop(out int i);
            parser.Stack.Pop(out double val);

            if (i < parser.Storage.Length)
            {
                parser.Storage[i] = val;
            }
        }

        private static void Op_Get(Parser parser)
        {
            parser.Stack.Pop(out int i);
            parser.Stack.Push(i < parser.Storage.Length
                ? parser.Storage[i]
                : 0);
        }

        #endregion

        #region Conditional operators

        private static void Op_And(Parser parser)
        {
            parser.Stack.Pop(out double num1, out double num2);
            parser.Stack.Push(num1 != 0 && num2 != 0 ? 1 : 0);
        }

        private static void Op_Or(Parser parser)
        {
            parser.Stack.Pop(out double num1, out double num2);
            parser.Stack.Push(num1 != 0 || num2 != 0 ? 1 : 0);
        }

        private static void Op_Not(Parser parser)
        {
            parser.Stack.Pop(out double num1);
            parser.Stack.Push(num1 != 0 ? 0 : 1);
        }

        private static void Op_Eq(Parser parser)
        {
            parser.Stack.Pop(out double num1, out double num2);
            parser.Stack.Push(num1 == num2 ? 1 : 0);
        }

        private static void Op_IfElse(Parser parser)
        {
            parser.Stack.Pop(out double v1, out double v2);
            parser.Stack.Pop(out double s1, out double s2);
            parser.Stack.Push(v1 <= v2 ? s1 : s2);
        }

        #endregion

        #region Subroutine operators

        private static void Op_CallSubr(Parser parser)
        {
            parser.Stack.Pop(out int number);

            parser.CallSubr(number, global: false);
        }

        private static void Op_CallGSubr(Parser parser)
        {
            parser.Stack.Pop(out int number);

            parser.CallSubr(number, global: true);
        }

        private static void Op_Return(Parser parser)
        {
            parser.Return();
        }

        #endregion

        #region Deprecated operators

        private static void Op_DotSection(Parser parser)
        {
            // Treat as a noop according to spec
        }

        #endregion

        #region Type 1 operators

        private static void Op_VStem3(Parser parser)
        {
            var startAt = parser.Stack.Count - 6;
            if (startAt < 0)
            {
                throw new CharStringStackUnderflowException();
            }

            // Make relative
            parser.Stack[startAt + 4] =
                parser.Stack[startAt + 4] - parser.Stack[startAt + 2] - parser.Stack[startAt + 3];

            parser.Stack[startAt + 2] =
                parser.Stack[startAt + 2] - parser.Stack[startAt + 0] - parser.Stack[startAt + 1];

            Op_VStem(parser);
        }


        private static void Op_HStem3(Parser parser)
        {
            var startAt = parser.Stack.Count - 6;
            if (startAt < 0)
            {
                throw new CharStringStackUnderflowException();
            }

            // Make relative
            parser.Stack[startAt + 4] =
                parser.Stack[startAt + 4] - parser.Stack[startAt + 2] - parser.Stack[startAt + 3];

            parser.Stack[startAt + 2] =
                parser.Stack[startAt + 2] - parser.Stack[startAt + 0] - parser.Stack[startAt + 1];

            Op_HStem(parser);
        }

        private static void Op_Pop(Parser parser)
        {
            if (parser.PostScriptStack.Count > 0)
            {
                parser.Stack.Push(parser.PostScriptStack.Pop());
            }
        }

        private static void Op_Hsbw(Parser parser)
        {
            parser.Stack.Pop(out double sbx, out double wx);

            parser.CharString.Width = wx;

            if (sbx != 0)
            {
                parser.CharString.Content.Add(CharStringLexeme.Operand(sbx));
                parser.CharString.Content.Add(CharStringLexeme.Operator(CharStringOpCode.HMoveTo));
                parser.Path.RMoveTo(sbx, 0);
            }
        }

        private static void Op_Seac(Parser parser)
        {
            parser.Stack.Pop(out int bchar, out int achar);
            parser.Stack.Pop(out double adx, out double ady);
            parser.Stack.Pop(out double asb);

            parser.CharString.Seac = new CharStringSeacInfo(adx, ady, bchar, achar);
        }

        private static void Op_Sbw(Parser parser)
        {
            parser.Stack.Pop(out double wx, out double wy);
            parser.Stack.Pop(out double sbx, out double sby);

            parser.CharString.Width = wx;

            if (sbx != 0 || sby != 0)
            {
                parser.CharString.Content.Add(CharStringLexeme.Operand(sbx));
                parser.CharString.Content.Add(CharStringLexeme.Operand(sby));
                parser.CharString.Content.Add(CharStringLexeme.Operator(CharStringOpCode.RMoveTo));
                parser.Path.RMoveTo(sbx, sby);
            }
        }

        private static void Op_CallOtherSubr(Parser parser)
        {
            // OtherSubrs are PostScript subroutines included in Type 1 fonts. The routines are however highly
            // standardized, so they are hardcoded in C# below to prevent having to implement the entire PostScript
            // language.
            //
            // What the routines do is not well documented, and the spec only include their source code in PostScript.
            // The routines below were instead created by looking at other implementations and a lot of trial and error.
            //
            // Reference implementations:
            // https://gitlab.freedesktop.org/freetype/freetype/-/blob/872a759b468ef0d88b0636d6beb074fe6b87f9cd/src/psaux/psintrp.c#L1704
            // https://github.com/mozilla/pdf.js/blob/96e34fbb7d7bb556392646a7a6720182953ac275/src/core/type1_parser.js#L260

            parser.Stack.Pop(out int n, out int othersubr);

            var args = new double[n];
            for (var i = 0; i < args.Length; i++)
            {
                parser.Stack.Pop(out args[args.Length - i - 1]);
            }

            switch ((CharStringOtherSubr)othersubr)
            {
                case CharStringOtherSubr.StartFlex:
                    parser.FlexPoints = new List<Point>(7);
                    break;

                case CharStringOtherSubr.AddFlexVector:
                    break;

                case CharStringOtherSubr.EndFlex:
                    var flexPoints = parser.FlexPoints;
                    parser.FlexPoints = null;

                    if (flexPoints?.Count == 7)
                    {
                        parser.Stack.Push(flexPoints[0].X + flexPoints[1].X);
                        parser.Stack.Push(flexPoints[0].Y + flexPoints[1].Y);

                        parser.Stack.Push(flexPoints[2].X);
                        parser.Stack.Push(flexPoints[2].Y);

                        parser.Stack.Push(flexPoints[3].X);
                        parser.Stack.Push(flexPoints[3].Y);

                        parser.Stack.Push(flexPoints[4].X);
                        parser.Stack.Push(flexPoints[4].Y);

                        parser.Stack.Push(flexPoints[5].X);
                        parser.Stack.Push(flexPoints[5].Y);

                        parser.Stack.Push(flexPoints[6].X);
                        parser.Stack.Push(flexPoints[6].Y);

                        var x3 = flexPoints[0].X + flexPoints[1].X + flexPoints[2].X + flexPoints[3].X;
                        var y3 = flexPoints[0].Y + flexPoints[1].Y + flexPoints[2].Y + flexPoints[3].Y;

                        var x6 = x3 + flexPoints[4].X + flexPoints[5].X + flexPoints[6].X;
                        var y6 = y3 + flexPoints[4].Y + flexPoints[5].Y + flexPoints[6].Y;

                        var flex = Math.Abs(x6 * y3 - y6 * x3) / Math.Sqrt(x6 * x6 + y6 * y6);

                        parser.Stack.Push(flex);

                        Op_Flex(parser);

                        parser.PostScriptStack.Push(parser.Path.LastY);
                        parser.PostScriptStack.Push(parser.Path.LastX);
                    }
                    break;

                case CharStringOtherSubr.ChangeHints:
                    if (args.Length < 1)
                    {
                        throw new ArgumentException("Too few arguments specified to othersubr 3.");
                    }

                    parser.PostScriptStack.Push(args[0]);

                    // Potential improvement:
                    // Right now all hints will be enabled at all time. A better approach would be to transform the Type 1
                    // stems to hstemhm/vstemh and turn on/off stems after calling othersubr 3. This would however require
                    // a complete rewrite of the hint handling. Type 1 fonts are pretty rare, so let's skip this for now.
                    break;
            }
        }


        private static void Op_ClosePath(Parser parser)
        {
            // Noop in type 2
        }

        private static void Op_SetCurrentPoint(Parser parser)
        {
            parser.Stack.Pop(out double x, out double y);

            var dx = x - parser.Path.LastX;
            var dy = y - parser.Path.LastY;

            if (dx != 0 || dy != 0)
            {
                // This is not entirely correct, but hopefully good enough.
                parser.CharString.Content.Add(CharStringLexeme.Operand(dx));
                parser.CharString.Content.Add(CharStringLexeme.Operand(dy));
                parser.CharString.Content.Add(CharStringLexeme.Operator(CharStringOpCode.RLineTo));
            }
        }

        #endregion
    }
}
