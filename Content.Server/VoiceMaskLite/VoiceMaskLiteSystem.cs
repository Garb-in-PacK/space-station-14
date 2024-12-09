using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Clothing;
using Content.Shared.Database;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Preferences;
using Content.Shared.Speech;
using Content.Shared.VoiceMask;
using Robust.Shared.Prototypes;

namespace Content.Server.VoiceMask;

public sealed partial class VoiceMaskLiteSystem : EntitySystem
{
    
    [Dependency] private readonly SharedActionsSystem _actions = default!;


    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VoiceMaskLiteComponent, InventoryRelayedEvent<TransformSpeakerNameEvent>>(OnTransformSpeakerName);
        SubscribeLocalEvent<VoiceMaskLiteComponent, ClothingGotEquippedEvent>(OnEquip);
        SubscribeLocalEvent<VoiceMaskLiteToggle>(TurnOnMask);
        
    }

    private void OnTransformSpeakerName(Entity<VoiceMaskLiteComponent> entity, ref InventoryRelayedEvent<TransformSpeakerNameEvent> args)
    {
        if ((entity.Comp.VoiceMaskToggler ?? false) == true)
        {
            args.Args.VoiceName = GetCurrentVoiceName(entity);
            args.Args.SpeechVerb = entity.Comp.VoiceMaskSpeechVerb ?? args.Args.SpeechVerb;
        }
    }


    private void OnEquip(EntityUid uid, VoiceMaskLiteComponent component, ClothingGotEquippedEvent args)
    {
        _actions.AddAction(args.Wearer, ref component.ActionEntity, component.Action, uid);
    }

    private void TurnOnMask(VoiceMaskLiteToggle ev)
    {
        var maskEntity = ev.Action.Comp.Container;

        if (!TryComp<VoiceMaskLiteComponent>(maskEntity, out var voiceMaskComp))
            return;

        if ((voiceMaskComp.VoiceMaskToggler ?? false) == true)
            voiceMaskComp.VoiceMaskToggler = false;
        else
            voiceMaskComp.VoiceMaskToggler = true;



    }

   

    #region Helper functions
    private string GetCurrentVoiceName(Entity<VoiceMaskLiteComponent> entity)
    {
        return Loc.GetString("voice-mask-default-name-override");
    }
    #endregion
}
