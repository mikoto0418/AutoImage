namespace SmartDiagram.Core.Diagram;

public enum DiagramType
{
    FunctionModule,
    Flowchart,
    ChenEr,
    Architecture,
    Hierarchy,
    Swimlane,
    Dfd,
    SimpleClass
}

public enum DiagramNodeType
{
    Module,
    Process,
    Decision,
    Start,
    End,
    Entity,
    Relationship,
    Attribute,
    Actor,
    Database,
    Class
}

public enum DiagramShape
{
    Rectangle,
    RoundedRectangle,
    Diamond,
    Ellipse,
    Parallelogram,
    Cylinder,
    Swimlane
}

public enum DiagramLineType
{
    Straight,
    Orthogonal
}

public enum DiagramArrow
{
    None,
    Classic
}
