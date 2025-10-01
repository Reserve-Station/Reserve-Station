



using Content.Shared.Implants;
using Content.Shared.Rejuvenate;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Mobs;
using Content.Server.Administration.Systems;
using Content.Server.Teleportation;
using Content.Shared.Teleportation;
using Content.Server.Mind;
using Content.Shared.Examine;


namespace Content.Server._Freakystation;

public sealed class SecondChanceSystem : EntitySystem
{
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly RejuvenateSystem _rejuvenate = default!;
    [Dependency] private readonly TeleportSystem _teleport = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SecondChanceComponent, ImplantImplantedEvent>(OnImplant);
        SubscribeLocalEvent<SecondChanceComponent, MobStateChangedEvent>(OnMobState);
        SubscribeLocalEvent<SecondChanceComponent, ImplantRemovedFromEvent>(OnUnimplanted);
        SubscribeLocalEvent<SecondChanceComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SecondChanceComponent, ExaminedEvent>(OnExamine);





    }



    private void OnComponentInit(EntityUid uid, SecondChanceComponent component, ComponentInit args)
    {
        _popup.PopupEntity(Loc.GetString("Вы живы... пока"), uid, uid);
    }

    private void OnImplant(EntityUid uid, SecondChanceComponent component, ImplantImplantedEvent args)
    {

        if (args.Implanted.HasValue)
            EnsureComp<SecondChanceComponent>(args.Implanted.Value);

        
 
    }


    private void OnMobState(EntityUid uid, SecondChanceComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead)
        {
            if (component.Uses > 0)
            {

     

                 var teleport = EnsureComp<RandomTeleportComponent>(uid);
                _teleport.RandomTeleport(uid, teleport);
                _rejuvenate.PerformRejuvenate(uid);




               
                _popup.PopupEntity(Loc.GetString("Чем выше шкаф, тем громче падает"), uid, uid);

                component.Uses--;

                var trymind = _mindSystem.TryGetMind(uid, out var mindId, out var mind);
                if (trymind)
                {
                    _mindSystem.TransferTo(mindId, uid, mind: mind);
                    _mindSystem.UnVisit(mindId);
                }

            }
            else
            {
                _popup.PopupEntity(Loc.GetString("Кажется... Это конец"), uid, uid);
            }




        }
    }

    public void OnUnimplanted(Entity<SecondChanceComponent> ent, ref ImplantRemovedFromEvent args)
    {
        if (HasComp<SecondChanceComponent>(args.Implant))
            RemComp<SecondChanceComponent>(ent);
    }




    private void OnExamine(EntityUid uid, SecondChanceComponent component, ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("[color=red]По телу пульсирует странная энергия[/color]"));
    }





}
