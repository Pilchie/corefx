// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal partial class ExpressionBinder
    {
        protected class UnaOpSig
        {
            public UnaOpSig()
            {
            }
            public UnaOpSig(PredefinedType pt, UnaOpMask grfuom, int cuosSkip, PfnBindUnaOp pfn, UnaOpFuncKind fnkind)
            {
                this.pt = pt;
                this.grfuom = grfuom;
                this.cuosSkip = cuosSkip;
                this.pfn = pfn;
                this.fnkind = fnkind;
            }
            public PredefinedType pt;
            public UnaOpMask grfuom;
            public int cuosSkip;
            public PfnBindUnaOp pfn;
            public UnaOpFuncKind fnkind;
        }

        protected class UnaOpFullSig : UnaOpSig
        {
            private LiftFlags _grflt;
            private CType _type;

            public UnaOpFullSig(CType type, PfnBindUnaOp pfn, LiftFlags grflt, UnaOpFuncKind fnkind)
            {
                this.pt = PredefinedType.PT_UNDEFINEDINDEX;
                this.grfuom = UnaOpMask.None;
                this.cuosSkip = 0;
                this.pfn = pfn;
                _type = type;
                _grflt = grflt;
                this.fnkind = fnkind;
            }
            /***************************************************************************************************
                Set the values of the UnaOpFullSig from the given UnaOpSig. The ExpressionBinder is needed to get
                the predefined type. Returns true iff the predef type is found.
            ***************************************************************************************************/
            public UnaOpFullSig(ExpressionBinder fnc, UnaOpSig uos)
            {
                this.pt = uos.pt;
                this.grfuom = uos.grfuom;
                this.cuosSkip = uos.cuosSkip;
                this.pfn = uos.pfn;
                this.fnkind = uos.fnkind;
                _type = pt != PredefinedType.PT_UNDEFINEDINDEX ? fnc.GetOptPDT(pt) : null;
                _grflt = LiftFlags.None;
            }
            public bool FPreDef()
            {
                return pt != PredefinedType.PT_UNDEFINEDINDEX;
            }
            public bool isLifted()
            {
                // This is a unary operator, so the second argument should be neither lifted nor converted.
                Debug.Assert((_grflt & LiftFlags.Lift2) == 0);
                Debug.Assert((_grflt & LiftFlags.Convert2) == 0);
                if (_grflt == LiftFlags.None)
                {
                    return false;
                }
                // We can't both convert and lift.
                Debug.Assert(((_grflt & LiftFlags.Lift1) == 0) || ((_grflt & LiftFlags.Convert1) == 0));
                return true;
            }
            public bool Convert()
            {
                return (_grflt & LiftFlags.Convert1) != 0;
            }

            public new CType GetType()
            {
                return _type;
            }
        }
    }
}
