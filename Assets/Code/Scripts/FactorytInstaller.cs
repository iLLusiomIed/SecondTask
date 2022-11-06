using UnityEngine;
using Zenject;

public class FactorytInstaller : MonoInstaller
{
    public GameController gameControl;
    public Field field;
    public ColorList colorList;
    public CellAnimationController cellAnimControl;
    public Cell cellPrefab;
    public CellAnimation cellAnimPrefab;
    public override void InstallBindings()
    {
        Container.Bind<ColorList>()
            .FromInstance(colorList)
            .AsSingle().NonLazy();

        Container.Bind<CellAnimationController>()
            .FromInstance(cellAnimControl)
            .AsSingle().NonLazy();

        Container.Bind<GameController>()
            .FromInstance(gameControl)
            .AsSingle().NonLazy();

        Container.Bind<Field>()
            .FromInstance(field)
            .AsSingle().NonLazy();

        Container.BindFactory<Transform, Cell, Cell.Factory>()
            .FromComponentInNewPrefab(cellPrefab);

        Container.BindFactory<Transform, CellAnimation, CellAnimation.Factory>()
            .FromComponentInNewPrefab(cellAnimPrefab);
    }
}