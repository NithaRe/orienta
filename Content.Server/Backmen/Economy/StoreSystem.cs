// ReSharper disable once CheckNamespace

using Content.Server.Access.Systems;
using Content.Server.Backmen.Economy;
using Content.Server.Store.Components;
using Content.Server.VendingMachines;
using Content.Shared.Backmen.Store;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Content.Shared.VendingMachines;

namespace Content.Server.Store.Systems;

public sealed partial class StoreSystem
{
    [Dependency] private readonly VendingMachineSystem _vendingMachineSystem = default!;

    private void _PlayDeny(EntityUid uid)
    {
        if (TryComp<VendingMachineComponent>(uid, out var vendingMachineComponent))
        {
            _vendingMachineSystem.Deny((uid,vendingMachineComponent));
        }
    }
    private void _PlayEject(EntityUid uid)
    {
        if (TryComp<VendingMachineComponent>(uid, out var vendComponent))
        {
            vendComponent.NextItemToEject = null;
            vendComponent.ThrowNextItem = false;
            _vendingMachineSystem.TryUpdateVisualState((uid, vendComponent));
            _audio.PlayPvs(vendComponent.SoundVend, uid);
        }
    }
    private bool HandleBankTransaction(EntityUid uid, StoreComponent component, StoreBuyListingMessage msg, ListingDataWithCostModifiers listing)
    {
        if (!TryComp<BuyStoreBankComponent>(uid, out var storeBank))
        {
            return false;
        }

        if (msg.Actor is not { Valid: true } buyer)
            return false;

        // Check that we have enough money
        foreach (var currency in listing.Cost)
        {
            if (!component.Balance.TryGetValue(currency.Key, out var balance) || balance < currency.Value)
            {
                _PlayDeny(uid);
                _popup.PopupEntity(Loc.GetString("store-no-money"), uid);
                return false;
            }
        }

        // Deduct the cost from the balance
        foreach (var currency in listing.Cost)
        {
            component.Balance[currency.Key] -= currency.Value;
        }

        return true; // Successfully deducted
    }
}
