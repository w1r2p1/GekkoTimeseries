﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gekko
{
    public class GekkoError : IVariable
    {
        public int underflow = 0;
        public int overflow = 0;        
        
        public GekkoError()
        {

        }

        public GekkoError(int underflow, int overflow)
        {
            this.underflow = underflow;
            this.overflow = overflow;
        }

        public IVariable Indexer(GekkoSmpl t, bool isLhs, params IVariable[] indexes)
        {
            throw new GekkoException();
        }

        public IVariable Indexer(GekkoSmpl t, IVariablesFilterRange indexRange)
        {
            throw new GekkoException();
        }

        public IVariable Indexer(GekkoSmpl t, IVariablesFilterRange indexRange1, IVariablesFilterRange indexRange2)
        {
            throw new GekkoException();
        }

        public IVariable Indexer(GekkoSmpl t, IVariable index, IVariablesFilterRange indexRange)
        {
            throw new GekkoException();
        }

        public IVariable Indexer(GekkoSmpl t, IVariablesFilterRange indexRange, IVariable index)
        {
            throw new GekkoException();
        }

        public IVariable Negate(GekkoSmpl t)
        {
            return null;
        }

        public void InjectAdd(GekkoSmpl t, IVariable x, IVariable y)
        {
            throw new GekkoException();
        }

        public double GetVal(GekkoSmpl t)
        {
            throw new GekkoException();
        }

        public string GetString()
        {
            throw new GekkoException();
        }

        public GekkoTime GetDate(O.GetDateChoices c)
        {
            throw new GekkoException();
        }

        public List<string> GetList()
        {
            throw new GekkoException();
        }

        public EVariableType Type()
        {
            return EVariableType.GekkoError;
        }

        public IVariable Add(GekkoSmpl t, IVariable x)
        {            
            throw new GekkoException();
        }

        public IVariable Subtract(GekkoSmpl t, IVariable x)
        {
            return null;
        }

        public IVariable Multiply(GekkoSmpl t, IVariable x)
        {
            return null;
        }

        public IVariable Divide(GekkoSmpl t, IVariable x)
        {
            return null;
        }

        public IVariable Power(GekkoSmpl t, IVariable x)
        {
            return null;
        }
    }
}
