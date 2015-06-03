/* Copyright (C) 2012-2015 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ISCSI
{
    public abstract class SCSICommandDescriptorBlock
    {
        public SCSIOpCodeName OpCode;
        public byte MiscellaneousCDBInformationHeader;
        public byte ServiceAction;
        public uint AdditionalCDBdata;
        public uint LogicalBlockAddress;
        public uint TransferLength; // number of blocks, also doubles as Parameter list length /  Allocation length
        public byte MiscellaneousCDBinformation;
        public byte Control;

        protected SCSICommandDescriptorBlock()
        { 
        }

        public abstract byte[] GetBytes();

        public static SCSICommandDescriptorBlock FromBytes(byte[] buffer, int offset)
        {
            byte opCode = buffer[offset + 0];
            switch ((SCSIOpCodeName)opCode)
            {
                case SCSIOpCodeName.TestUnitReady:
                    return new SCSICommandDescriptorBlock6(buffer, offset);
                case SCSIOpCodeName.RequestSense:
                    return new SCSICommandDescriptorBlock6(buffer, offset);
                case SCSIOpCodeName.Read6:
                    return new SCSICommandDescriptorBlock6(buffer, offset);
                case SCSIOpCodeName.Write6:
                    return new SCSICommandDescriptorBlock6(buffer, offset);
                case SCSIOpCodeName.Inquiry:
                    return new InquiryCommand(buffer, offset);
                case SCSIOpCodeName.Reserve6:
                    return new SCSICommandDescriptorBlock6(buffer, offset);
                case SCSIOpCodeName.Release6:
                    return new SCSICommandDescriptorBlock6(buffer, offset);
                case SCSIOpCodeName.ModeSense6:
                    return new ModeSense6CommandDescriptorBlock(buffer, offset);
                case SCSIOpCodeName.ReadCapacity10:
                    return new SCSICommandDescriptorBlock10(buffer, offset);
                case SCSIOpCodeName.Read10:
                    return new SCSICommandDescriptorBlock10(buffer, offset);
                case SCSIOpCodeName.Write10:
                    return new SCSICommandDescriptorBlock10(buffer, offset);
                case SCSIOpCodeName.Verify10:
                    return new SCSICommandDescriptorBlock10(buffer, offset);
                case SCSIOpCodeName.SynchronizeCache10:
                    return new SCSICommandDescriptorBlock10(buffer, offset);
                case SCSIOpCodeName.WriteSame10:
                    return new SCSICommandDescriptorBlock10(buffer, offset);
                case SCSIOpCodeName.Read16:
                    return new SCSICommandDescriptorBlock16(buffer, offset);
                case SCSIOpCodeName.Write16:
                    return new SCSICommandDescriptorBlock16(buffer, offset);
                case SCSIOpCodeName.Verify16:
                    return new SCSICommandDescriptorBlock16(buffer, offset);
                case SCSIOpCodeName.WriteSame16:
                    return new SCSICommandDescriptorBlock16(buffer, offset);
                case SCSIOpCodeName.ServiceActionIn:
                    return new SCSICommandDescriptorBlock16(buffer, offset);
                case SCSIOpCodeName.ReportLUNs:
                    return new SCSICommandDescriptorBlock12(buffer, offset);
                default:
                    throw new UnsupportedSCSICommandException(String.Format("Unknown SCSI command: 0x{0}", opCode.ToString("x")));
            }
        }

        public ulong LogicalBlockAddress64
        {
            get
            {
                if (this is SCSICommandDescriptorBlock16)
                {
                    ulong result = (ulong)this.LogicalBlockAddress << 32;
                    result += this.AdditionalCDBdata;
                    return result;
                }
                else
                {
                    return this.LogicalBlockAddress;
                }
            }
        }
    }
}