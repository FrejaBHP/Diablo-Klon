using Godot;
using System;

public partial class SupportGem : InventoryItem {
    // Support Gems skal skrives, så de kan tilføje deres værdier direkte. Dvs hvis det er et support til projektiler, skal den kunne sige fx
    // Skill Gem -> IProjectileSkill -> AddedProjectiles
    // Dette vil undgå at fucke rundt med en million tjek i et dictionary
    // Logikken til at tjekke, hvornår et Support bliver tilføjet og fjernet, er sådan set fin nok, men resten er skrald og jeg hader det

    // I modsætning til det foroven, kommer det nok mere sandsynligt til at tage ActiveDamageModifiers som en ref parameter, som den så kan lave om på.
    // Så skal nok have nogle flags til at have Skill Gem bestemme, om den skal gå igennem med sådan en overførsel eller ej
}
