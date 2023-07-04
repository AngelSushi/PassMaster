using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryBox : Box {

    public override void BoxInteract(GameObject current,ChiefController controller) {
        currentController = controller;

        
        CookController.Team team = _cookController.teams.Where(t => t.players.Contains(currentController.gameObject)).ToList()[0];
        team.reputation -= 0.1f;
        Debug.Log("change reputation of team " + team.name + " to "+ team.reputation);
        
        if (current != null) {
            if (current.TryGetComponent<Plate>(out Plate plate)) {
                if (plate.fullRecipe != null) {
                    if (!plate.fullRecipe.NeedToBeCook || (plate.fullRecipe.NeedToBeCook && plate.fullRecipe.IsCooked)) 
                        Put();
                }
            }
        }        
    }

    protected override void Put() {
        RecipeController.Recipe targetRecipe = currentController.ActualPlate.GetComponent<Plate>().fullRecipe;
        CookController.Team targetTeam = _cookController.teams.Where(team => team.players.Contains(currentController.gameObject)).ToList()[0];

        if (targetTeam.HasRecipe(targetRecipe.Name)) {

            int currentPoint = (int) (_cookController.maxPointPerRecipe - _cookController.maxPointPerRecipe * (1 - (targetRecipe.Ticker.CurrentTime / targetRecipe.RecipeTime)));
            _cookController.AddPoint(currentPoint,currentController.gameObject);

            targetTeam.DeliverRecipe(targetRecipe);
            Destroy(currentController.ActualPlate);
        }
    }

    protected override void Take() { }

}
