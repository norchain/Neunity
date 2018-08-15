
using System.Numerics;

#if NEOSC
using Neunity.Adapters.NEO;
#else
using Neunity.Adapters.Unity;
#endif


namespace Neunity.Tools
{
    public static class NuSD
    {

        /* -----------------------------------------------------------------
         * Seralization/Deseralization
         * 
         * 
         * 
         *      Body   =====Seg=====>    Segment      ===Join===> Table (combination of segs without further pre.)
         *    [body]   <====DeSeg====    [pre][body] <===Dequeue==== [pre1][body1][pre2][[pre2.1][body2.1][pre2.2][body2.2]]
         *                     
         * 
         * "Pre" represents body's length
         -----------------------------------------------------------------*/

        #region Length
        public static int PreLenOfBody(byte[] body) => body.Length / 255 + 1;
        public static int SegLenOfBody(byte[] body) => PreLenOfBody(body) + body.Length;

        public static int PreLenOfSeg(byte[] segment) => PreLenOfFirstSegFromTable(segment); //A table can be just one segment
        public static int PreLenOfFirstSegFromTable(byte[] table) => PreLenOfFirstSegFromData(table, 0);
        public static int PreLenOfFirstSegFromData(byte[] data, int segStartIndex)
        {
            int i = 0;
            while (data[segStartIndex + i++] == 255) { }
            return i;
        }

        public static int BodyLenOfSeg(byte[] segment) => BodyLenOfFirstSegFromTable(segment);
        public static int BodyLenOfFirstSegFromTable(byte[] table) => BodyLenOfFirstSegFromData(table, 0);
        public static int BodyLenOfFirstSegFromData(byte[] data, int segStartIndex)
        {
            int prelen = PreLenOfFirstSegFromData(data, segStartIndex);
            return (prelen - 1) * 255 + data[prelen + segStartIndex - 1];
        }

        public static int SegLenOfFirstSegFromTable(byte[] table) => SegLenOfFirstSegFromData(table, 0);
        public static int SegLenOfFirstSegFromData(byte[] data, int segStartIndex) {
            int prelen = PreLenOfFirstSegFromData(data, segStartIndex);
            return prelen + (prelen - 1) * 255 + data[prelen + segStartIndex - 1];
        } 


        #endregion

        #region Counts

        public static int NumSegsOfTable(byte[] table) => NumSegsOfTableFromData(table, 0);
        public static int NumSegsOfSeg(byte[] segment) => NumSegsOfSegFromData(segment, 0);
        public static int NumSegsOfSegFromData(byte[] data, int segStartIndex = 0) => NumSegsOfTableFromData(data, PreLenOfFirstSegFromData(data, segStartIndex) + segStartIndex);

        public static int NumSegsOfTableFromData(byte[] data, int tblStartIndex = 0)
        {
            int i = tblStartIndex;
            int r = 0;
            while (i < data.Length)
            {
                i += SegLenOfFirstSegFromData(data, i);
                ++r;
            }
            return r;
        }


        #endregion

        #region Seg

        public static byte[] SegInt(BigInteger body) => Seg(Op.BigInt2Bytes(body));
        public static byte[] SegString(string body) => Seg(Op.String2Bytes(body));
        public static byte[] SegBool(bool body) => Seg(Op.Bool2Bytes(body));
        //public static byte[] SegByte(byte b) => new byte[2] { 1, b };

        public static byte[] Seg(byte[] body)
        {   //if body = Op.Void: rem = 0;

            BigInteger rem = (body.Length) % 255;

            byte[] r = Op.Void;

            for (int i = 0; i < body.Length / 255; i++)
            {
                r = Op.JoinTwoByteArray(r, new byte[1] { 255 });
            }

            return Op.JoinTwoByteArray(Op.JoinTwoByteArray(r, Op.BigInt2Bytes(rem)), body);
        }


        #endregion

        #region Join
        public static byte[] JoinSegs2Seg(params byte[][] segs) => Seg(JoinSegs2Table(segs));

        public static byte[] JoinSegs2Table(params byte[][] segs) => Op.JoinByteArray(segs);

        public static byte[] JoinToTable(byte[] table, byte[] seg) => Op.JoinTwoByteArray(table, seg);

        #endregion

        #region Deseg

        public static byte[] Deseg(byte[] seg) => DesegFromTable(seg);   //Seg is a table with only 1 seg
        public static byte[] DesegFromTable(byte[] table) => DesegFromTableFromData(table, 0);

        public static byte[] DesegFromTableFromData(byte[] data, int start)
        {
            if(start>= data.Length){
                return Op.Void;
            }
            else{
                int i = 0;  //prefixLength -1
                int segLen = 0;
                while (data[i + start] == 255)
                {
                    segLen += 255;
                    ++i;
                }
                segLen += data[i + start];
                if (segLen == 0)
                {
                    return Op.Void;
                }
                else
                {
                    return Op.SubBytes(data, start + i + 1, segLen);
                }
            }


        }


        //Get the value (as byte[]) with the variable id w/o having all previous segments deseged.
        public static byte[] DesegWithIdFromTable(byte[] table, int id) => DesegWithIdFromData(table, 0, id);
        public static byte[] DesegWithIdFromSeg(byte[] segment, int id) => DesegWithIdFromData(segment, PreLenOfSeg(segment), id);

        public static byte[] DesegWithIdFromData(byte[] data, int tblStartIndex, int id)
        {
            int i = 0;
            int preStart = tblStartIndex;
            while (i < id)
            {
                preStart += SegLenOfFirstSegFromData(data, preStart);
                if (preStart > data.Length)
                {
                    return Op.Void;
                }
                ++i;
            }
            return DesegFromTableFromData(data, preStart);

        }
        #endregion

        #region Standard

        public static byte[] AddSeg(this byte[] data, byte[] body) => Op.JoinTwoByteArray(data, Seg(body));
        public static byte[] AddSegInt(this byte[] data, BigInteger body) => Op.JoinTwoByteArray(data, SegInt(body));
        public static byte[] AddSegBool(this byte[] data, bool body) => Op.JoinTwoByteArray(data, SegBool(body));
        public static byte[] AddSegStr(this byte[] data, string body) => Op.JoinTwoByteArray(data, SegString(body));

        public static byte[] AddBody(this byte[] data, byte[] body) => Op.JoinTwoByteArray(data, body);
        public static byte[] AddBodyInt(this byte[] data, BigInteger body) => Op.JoinTwoByteArray(data, SegInt(body));
        public static byte[] AddBodyBool(this byte[] data, bool body) => Op.JoinTwoByteArray(data, SegBool(body));
        public static byte[] AddBodyStr(this byte[] data, string body) => Op.JoinTwoByteArray(data, SegString(body));

        public static byte[] SplitSeg(this byte[] data, int startID) => DesegFromTableFromData(data, startID);
        public static BigInteger SplitSegInt(this byte[] data, int startID) => Op.Bytes2BigInt(DesegFromTableFromData(data, startID));
        public static bool SplitSegBool(this byte[] data, int startID) => Op.Bytes2Bool(DesegFromTableFromData(data, startID));
        public static string SplitSegStr(this byte[] data, int startID) => Op.Bytes2String(DesegFromTableFromData(data, startID));

        public static byte[] SplitBody(this byte[] data, int startID, int length) => Op.SubBytes(data,startID,length);

        public static byte[] SplitTbl(this byte[] table, int index) => DesegWithIdFromData(table, 0, index);
        public static BigInteger SplitTblInt(this byte[] table, int index) => Op.Bytes2BigInt(DesegWithIdFromData(table, 0, index));
        public static bool SplitTblBool(this byte[] table, int index) => Op.Bytes2Bool(DesegWithIdFromData(table, 0, index));
        public static string SplitTblStr(this byte[] table, int index) => Op.Bytes2String(DesegWithIdFromData(table, 0, index));
        public static int SizeTable(this byte[] table) => NumSegsOfTableFromData(table, 0);

        #endregion
    }


}
