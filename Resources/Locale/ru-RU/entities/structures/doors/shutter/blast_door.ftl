ent-BlastDoor = гермозатвор
    .desc = Имеется надпись "ОПАСНОСТЬ ВЗРЫВА".
ent-BlastDoorOpen = { ent-BlastDoor }
    .suffix = Открытый
    .desc = { ent-BlastDoor.desc }
ent-BlastDoorFrame = каркас гермозатвора
    .desc = { ent-BlastDoor.desc }

ent-BlastDoorCentralCommand = { ent-BlastDoor }
    .desc = { ent-BlastDoor.desc }
    .suffix = ЦентКом, Закрыт
ent-BlastDoorOpenCentralCommand = { ent-BlastDoorCentralCommand }
    .desc = { ent-BlastDoorCentralCommand.desc }
    .suffix = Открытый, { ent-BlastDoorCentralCommand.suffix }

ent-BlastDoorXeno = ксеногермозатвор
    .desc = Не потеряйте руку!
    .suffix = Ксеноморф
ent-BlastDoorXenoOpen = { ent-BlastDoorXeno }
    .desc = { ent-BlastDoorXeno.desc }
    .suffix = Ксеноморф, Открытый
ent-BlastDoorXenoFrame = каркас ксеногермозатвора
    .desc = { ent-BlastDoorXeno.desc }
    .suffix = Ксеноморф

ent-BlastDoorXenoborg = { ent-BlastDoor }
    .desc = Не потеряйте руку!
    .suffix = Ксеноборг
ent-BlastDoorXenoborgOpen = { ent-BlastDoorXenoborg }
    .desc = { ent-BlastDoorXenoborg.desc }
    .suffix = Ксеноборг, Открытый
