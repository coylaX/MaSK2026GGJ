using System.Collections.Generic;
using UnityEngine;
using static TMPro.Examples.ObjectSpin;


#region 3. ��Ӫ�������� (Inventory)
// ==========================================
// �������ϡ���������������ݽṹ
// ==========================================
[System.Serializable]
public class MorningInventoryData
{
    public EmotionTraitID emotionTraitID;
    public MemoryTraitID memoryTraitID;

    // --- ��Դ A: ���� ---
    public int pigmentAmount;

    // --- ��Դ B: ���� (�����������) ---
    public int xiCount;
    public int nuCount;
    public int aiCount;
    public int leCount;

    // --- ��Դ C: ������� (ǰ���ǻ�õļ��䣬�����ǹؿ���ǰ�׶εļ���) ---
    public List<MemoryTraitID> memoryGet;
    public MemoryTraitID memoryNight;

    public MorningInventoryData()
    {
        pigmentAmount = 500;
        xiCount = 2;
        nuCount = 2;
        aiCount = 2;
        leCount = 2;
        memoryGet = new List<MemoryTraitID>();
        memoryNight = MemoryTraitID.A;
    }
}
#endregion

#region 4. ���������� (OrderData) - �����
[System.Serializable]
public class OrderData
{
    public string orderID;      // ��Ӧ OrderTemplate ��Ψһ ID
    public int daysRemaining;   // ״̬��ʶ��>0 ʣ������, -1 ����ɺ���, -2 �ѹ��ڡ�-3����ɲ���

    public OrderData(string id, int days)
    {
        this.orderID = id;
        this.daysRemaining = days;
    }
}
#endregion