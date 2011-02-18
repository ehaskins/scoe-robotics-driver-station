﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHaskins.Frc.Communication
{
    public class Mode : BitField8
    {
        public bool FpgaChecksum
        {
            get { return this[0]; }
            set
            {
                this[0] = value;
            }
        }
        public bool CRioChecksum
        {
            get { return this[1]; }
            set
            {
                this[1] = value;
            }
        }
        public bool Resync
        {
            get { return this[2]; }
            set
            {
                this[2] = value;
            }
        }
        public bool Autonomous
        {
            get { return this[4]; }
            set
            {
                this[4] = value;
            }
        }
        public bool Enabled
        {
            get { return this[5]; }
            set
            {
                this[5] = value;
            }
        }
        public bool EStop
        {
            get { return !this[6]; }
            set
            {
                this[6] = !value;
            }
        }
        public bool Reset
        {
            get { return this[7]; }
            set
            {
                this[7] = value;
            }
        }
    }
}