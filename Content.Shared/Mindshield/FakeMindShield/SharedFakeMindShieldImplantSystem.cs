using Content.Shared.Actions;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Mindshield.Components;

namespace Content.Shared.Mindshield.FakeMindShield;

public sealed class SharedFakeMindShieldImplantSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SubdermalImplantComponent, FakeMindShieldToggleEvent>(OnFakeMindShieldToggle);
        SubscribeLocalEvent<FakeMindShieldImplantComponent, ImplantImplantedEvent>(ImplantCheck);
        SubscribeLocalEvent<FakeMindShieldComponent, ImplantRemovedFromEvent>(ImplantRemove); // Goob fix
    }
    /// <summary>
    /// Raise the Action of a Implanted user toggling their implant to the FakeMindshieldComponent on their entity
    /// </summary>
    private void OnFakeMindShieldToggle(Entity<SubdermalImplantComponent> entity, ref FakeMindShieldToggleEvent ev)
    {
        ev.Handled = true;
        if (entity.Comp.ImplantedEntity is not { } ent)
            return;

        if (!TryComp<FakeMindShieldComponent>(ent, out var comp))
            return;
        _actionsSystem.SetToggled(ev.Action, !comp.IsEnabled); // Set it to what the Mindshield component WILL be after this
        RaiseLocalEvent(ent, ev); //this reraises the action event to support an eventual future Changeling Antag which will also be using this component for it's "mindshield" ability
    }
    private void ImplantCheck(EntityUid uid, FakeMindShieldImplantComponent component, ref ImplantImplantedEvent ev)
    {
        if (ev.Implanted != null)
            EnsureComp<FakeMindShieldComponent>(ev.Implanted.Value);
    }

    // Goob edit - removable mindshields, parity/anti-metagame fix. FakeMindShield instead of FakeMindShieldImplant since the latter doesn't trigger
    private void ImplantRemove(EntityUid uid, FakeMindShieldComponent component, ref ImplantRemovedFromEvent ev)
    {
        if (HasComp<FakeMindShieldImplantComponent>(ev.Implant))
            RemComp<FakeMindShieldComponent>(ev.Implanted);
    }
}
