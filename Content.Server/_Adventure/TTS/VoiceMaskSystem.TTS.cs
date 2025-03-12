using Content.Server._Adventure.TTS;
using Content.Shared.VoiceMask;
using Content.Shared._Adventure.TTS;
using Content.Shared.Inventory;

namespace Content.Server.VoiceMask;

public partial class VoiceMaskSystem
{
    private void InitializeTTS()
    {
        SubscribeLocalEvent<VoiceMaskComponent, InventoryRelayedEvent<TransformSpeakerVoiceEvent>>(OnSpeakerVoiceTransform);
        SubscribeLocalEvent<VoiceMaskComponent, VoiceMaskChangeVoiceMessage>(OnChangeVoice);
    }

    private void OnSpeakerVoiceTransform(Entity<VoiceMaskComponent> ent, ref InventoryRelayedEvent<TransformSpeakerVoiceEvent> args)
    {
        args.Args.VoiceId = ent.Comp.VoiceId;
    }

    private void OnChangeVoice(Entity<VoiceMaskComponent> entity, ref VoiceMaskChangeVoiceMessage msg)
    {
        if (msg.Voice is { } id && !_proto.HasIndex<TTSVoicePrototype>(id))
            return;

        entity.Comp.VoiceId = msg.Voice;

        _popupSystem.PopupEntity(Loc.GetString("voice-mask-voice-popup-success"), entity);

        UpdateUI(entity);
    }
}
