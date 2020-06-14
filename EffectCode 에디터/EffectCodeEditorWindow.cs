using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public enum EffectCodeValueType
{
    SecureInt,
    SecureFloat
}

public enum EffectCodeTriggerType
{
    Common,
    SAP,
    SC,
    SAC
}

public enum EffectCodeType
{
    Common,
    Buff,
    Debuff
}

public class EffectCodeStatInfo
{
    public EffectCodeValueType valueType;
    public EffectCodeTriggerType triggerType;

    public EffectCodeStatInfo(EffectCodeValueType valueType, EffectCodeTriggerType triggerType)
    {
        this.valueType = valueType;
        this.triggerType = triggerType;
    }
}

public class EffectCodeInfo
{
    public string effectCodeName;
    public EffectCodeType effectCodeType;
    public int statCount;
    public List<EffectCodeStatInfo> statList = new List<EffectCodeStatInfo>();

    public EffectCodeInfo(string effectCodeName, EffectCodeType effectCodeType)
    {
        this.effectCodeName = effectCodeName;
        this.effectCodeType = effectCodeType;
        this.statCount = 0;
        this.statList.Clear();
    }
}

public class EffectCodeEditorWindow : EditorWindow
{
    EffectCodeInfo effectCodeInfo;

    [MenuItem("EffectCode/EffectCodeEditor")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<EffectCodeEditorWindow>();
        window.titleContent = new GUIContent("EffectCodeEditor");
    }

    private void OnEnable()
    {
        if (effectCodeInfo == null)
        {
            effectCodeInfo = new EffectCodeInfo(string.Empty, EffectCodeType.Common);
        }
    }

    private void OnGUI()
    {
        effectCodeInfo.effectCodeName = EditorGUILayout.TextField("EffectCode Number", effectCodeInfo.effectCodeName, GUILayout.Width(400));
        GUILayout.Space(5);
        effectCodeInfo.effectCodeType = (EffectCodeType)EditorGUILayout.EnumPopup("EffectCode Type", effectCodeInfo.effectCodeType, GUILayout.Width(400));
        GUILayout.Space(5);
        effectCodeInfo.statCount = EditorGUILayout.IntField("Stat Count", effectCodeInfo.statCount, GUILayout.Width(400));
        GUILayout.Space(5);
        
        if (effectCodeInfo.statCount != effectCodeInfo.statList.Count)
        {
            effectCodeInfo.statList.Clear();
            for (int i = 0; i < effectCodeInfo.statCount; i++)
            {
                effectCodeInfo.statList.Add(new EffectCodeStatInfo(EffectCodeValueType.SecureInt, EffectCodeTriggerType.Common));
            }
        }

        if(effectCodeInfo.statList.Count > 0)
        {
            for(int i = 0; i < effectCodeInfo.statList.Count; i++)
            {
                var stat = effectCodeInfo.statList[i];
                EditorGUILayout.LabelField($"Stat {i + 1}");
                stat.valueType = (EffectCodeValueType)EditorGUILayout.EnumPopup("Value Type", stat.valueType, GUILayout.Width(400));
                stat.triggerType = (EffectCodeTriggerType)EditorGUILayout.EnumPopup("Trigger Type", stat.triggerType, GUILayout.Width(400));
                GUILayout.Space(5);
            }
        }

        if(GUILayout.Button("Create"))
        {
            string dirPath = Application.dataPath + "/" + "Scripts/EffectCodes/" + "EffectCode";
            DirectoryInfo di = new DirectoryInfo(dirPath);

            if(di.Exists == false)
            {
                di.Create();
                AssetDatabase.Refresh();
            }

            var scriptTxt = $"using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class EffectCode{effectCodeInfo.effectCodeName} : MonoBehaviour\n{{\n\tpublic void Initialize()\n\t{{\n\n\t}}\n}}";
            var scriptPath = dirPath + "/" + $"EffectCode{effectCodeInfo.effectCodeName}.cs";
            if (File.Exists(scriptPath))
            {
                Debug.LogError("해당 디렉토리에 이미 같은 이름의 이펙트코드가 존재합니다.");
                return;
            }

            Debug.Log($"EffectCode{effectCodeInfo.effectCodeName} 생성!");
            File.WriteAllText(scriptPath, scriptTxt);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
