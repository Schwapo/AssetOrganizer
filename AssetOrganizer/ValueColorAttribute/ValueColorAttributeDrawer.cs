#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEngine;

[DrawerPriority(0.4, 0.0, 0.0)]
public class ValueColorAttributeDrawer : OdinAttributeDrawer<ValueColorAttribute>
{
    private ValueResolver<Color> colorResolver;

    protected override void Initialize()
        => colorResolver = ValueResolver.Get(Property, Attribute.GetColor, Attribute.Color);

    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (colorResolver.HasError)
        {
            SirenixEditorGUI.ErrorMessageBox(colorResolver.ErrorMessage);
            CallNextDrawer(label);
        }
        else
        {
            var valueColor = colorResolver.GetValue();
            var valueColorInverse = new Color(
                1f / valueColor.r,
                1f / valueColor.g,
                1f / valueColor.b,
                1f / valueColor.a);

            GUIHelper.PushColor(valueColor);
            GUIHelper.PushContentColor(valueColorInverse);
            CallNextDrawer(label);
            GUIHelper.PopContentColor();
            GUIHelper.PopColor();
        }
    }
}
#endif