﻿using System;
using System.Collections.Generic;

namespace Surrogates.Applications.Tests
{
    [Serializable]
    public class Simple
    {
        public Simple() { }
        public Simple(List<int> list)
        {
            this.List = list;
        }

        public virtual List<int> List { get; set; }

        public virtual int GetFromList(int index)
        {
            return List[index];
        }

        public virtual void Add2List(int val)
        {
            List.Add(val);
        }

        public virtual void Set(string text)
        {
            this.Text = text;
        }

        public virtual string GetDomainName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }

        public string Text { get; set; }

        Random _rnd = new Random();
        public virtual int GetRandom()
        {
            return _rnd.Next();
        }

        public virtual object GetNewObject()
        {
            return new object();
        }
    }
}
