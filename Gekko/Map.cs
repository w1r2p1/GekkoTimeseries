﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gekko
{
    public class Map : IVariable, IBank
    {
        //Abstract class containing a List
        //Used for pointing to Lists without having to create/clone them.      
               

        public GekkoDictionary<string, IVariable> storage = null;

        public Map()
        {
            this.storage = new GekkoDictionary<string, IVariable>(StringComparer.OrdinalIgnoreCase);
        }

        public Map(GekkoDictionary<string, IVariable> map)
        {
            this.storage = map;
        }                

        //!!!This has nothing to do #m1+#m2 etc., see Add(GekkoSmpl t, IVariable x) instead.
        //   This method is just to avoid x.list.Add(...)
        public void Add(string s, IVariable x)
        {
            if (this.storage == null) this.storage = new GekkoDictionary<string, IVariable>(StringComparer.OrdinalIgnoreCase);
            this.storage.Add(s, x);
        }

        public int Count()
        {
            return this.storage.Count;
        }

        public IVariable GetIVariable(string variable)
        {
            IVariable iv = null;
            this.storage.TryGetValue(variable, out iv);
            return iv;
        }

        public bool ContainsIVariable(string variable)
        {            
            return storage.ContainsKey(variable);         
        }

        public void AddIVariableWithOverwrite(string name, IVariable x)
        {            
            if (this.ContainsIVariable(name))
            {
                this.RemoveIVariable(name);
                this.AddIVariable(name, x);
            }
        }

        public void AddIVariable(string name, IVariable x)
        {
            //Much simpler than the corresponding method in Databank.cs
            this.storage.Add(name, x);
        }

        public void RemoveIVariable(string name)
        {
            if (this.storage.ContainsKey(name)) this.storage.Remove(name);
        }

        public IVariable Indexer(GekkoSmpl t, params IVariable[] indexes)
        {
            if (indexes.Length == 1)
            {
                IVariable index = indexes[0];
                //Indices run from 1, 2, 3, ... n. Element 0 is length of list.
                if (index.Type() == EVariableType.String)
                {
                    string s = (index as ScalarString)._string2;
                    IVariable rv = null; this.storage.TryGetValue(s, out rv);
                    if (rv == null)
                    {
                        G.Writeln2("*** ERROR: The MAP does not contain the name '" + s + "'");
                        throw new GekkoException();
                    }
                    return rv;
                }                
                else
                {
                    G.Writeln2("*** ERROR: Type mismatch regarding MAP []-index (only STRING is allowed)");
                    throw new GekkoException();
                }
            }
            else
            {
                G.Writeln2("*** ERROR: Cannot use " + indexes.Length + "-dimensional []-index on MAP");
                throw new GekkoException();
            }
        }
        
        public IVariable Negate(GekkoSmpl t)
        {
            G.Writeln2("*** ERROR: You cannot use minus with MAP");
            throw new GekkoException();
        }

        public void InjectAdd(GekkoSmpl t, IVariable x, IVariable y)
        {
            G.Writeln2("*** ERROR: #8703458724");
            throw new GekkoException();
        }

        public double GetValOLD(GekkoSmpl t)
        {
            G.Writeln2("*** ERROR: Type mismatch: you are trying to extract a VAL from a MAP.");
            throw new GekkoException();
        }

        public double GetVal(GekkoTime t)
        {
            G.Writeln2("*** ERROR: Type mismatch: you are trying to extract a VAL from a MAP.");
            throw new GekkoException();
        }

        public double ConvertToVal()
        {
            G.Writeln2("*** ERROR: Cannot extract a VAL from " + G.GetTypeString(this) + " type");
            throw new GekkoException();
        }

        public string ConvertToString()
        {
            G.Writeln2("*** ERROR: Trying to convert a MAP into a STRING.");
            throw new GekkoException();
        }

        public GekkoTime ConvertToDate(O.GetDateChoices c)
        {
            G.Writeln2("*** ERROR: Type mismatch: you are trying to extract a DATE from a MAP.");
            throw new GekkoException();
        }

        public List<IVariable> ConvertToList()
        {
            G.Writeln2("*** ERROR: Type mismatch: you are trying to extract a LIST from a MAP.");
            throw new GekkoException();
        }

        public EVariableType Type()
        {
            return EVariableType.Map;
        }

        public IVariable Add(GekkoSmpl t, IVariable x)
        {
            G.Writeln2("*** ERROR: You cannot use add with MAPs");
            throw new GekkoException();
        }

        public IVariable Subtract(GekkoSmpl t, IVariable x)
        {
            G.Writeln2("*** ERROR: You cannot use subtract with MAPs");
            throw new GekkoException();
        }

        public IVariable Multiply(GekkoSmpl t, IVariable x)
        {
            G.Writeln2("*** ERROR: You cannot use multiply with MAPs");
            throw new GekkoException();
        }

        public IVariable Divide(GekkoSmpl t, IVariable x)
        {
            G.Writeln2("*** ERROR: You cannot use divide with MAPs");
            throw new GekkoException();
        }

        public IVariable Power(GekkoSmpl t, IVariable x)
        {
            G.Writeln2("*** ERROR: You cannot use power function with MAPs");
            throw new GekkoException();
        }

        public string Message()
        {
            return "map";
        }

        public void IndexerSetData(GekkoSmpl smpl, IVariable rhsExpression, params IVariable[] dims)
        {
            if (dims.Length == 1 && dims[0].Type() == EVariableType.String)
            {
                string s = O.ConvertToString(dims[0]);
                string dbName, varName, freq; O.Chop(s, out dbName, out varName, out freq);        
                if(dbName!=null)
                {
                    G.Writeln2("*** ERROR: You cannot state bank name (with colon) on the left-hand side in a MAP element");
                    throw new GekkoException();
                }        
                O.LookupHelperLeftside(smpl, this, s, freq, rhsExpression);
                
                //IVariable iv = this.GetIVariable(s);
                //if (iv != null) this.RemoveIVariable(s);
                //this.AddIVariable(s, rhsExpression);                
            }
            else
            {
                G.Writeln2("*** ERROR: Unexpected indexer type on MAP (left-hand side)");
                throw new GekkoException();
            }
        }

        public IVariable DeepClone()
        {
            Map temp = new Map();
            temp.storage = new GekkoDictionary<string, IVariable>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, IVariable> kvp in this.storage)
            {
                temp.storage.Add(kvp.Key, kvp.Value.DeepClone());
            }
            return temp;
        }

        public void DeepTrim()
        {            
            foreach (KeyValuePair<string, IVariable> kvp in this.storage)
            {
                kvp.Value.DeepTrim();
            }            
        }        
    }
}