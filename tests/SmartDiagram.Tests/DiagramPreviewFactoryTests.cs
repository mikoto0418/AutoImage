using System.Threading;
using System.Windows.Controls;
using SmartDiagram.App.Components;
using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Tests;

public sealed class DiagramPreviewFactoryTests
{
    [Fact]
    public void FromDocument_renders_nodes_and_edge_labels_from_current_document()
    {
        RunOnSta(() =>
        {
            var document = DiagramDocument.Create(
                "preview_dynamic_test",
                DiagramType.Flowchart,
                "Dynamic preview",
                [
                    new DiagramNode("draft", "论文草稿", DiagramNodeType.Process, DiagramShape.RoundedRectangle),
                    new DiagramNode("review", "导师审核", DiagramNodeType.Decision, DiagramShape.Diamond),
                    new DiagramNode("finish", "定稿归档", DiagramNodeType.End, DiagramShape.Ellipse)
                ],
                [
                    new DiagramEdge("edge_draft_review", "draft", "review", "提交", DiagramLineType.Orthogonal, DiagramArrow.Classic),
                    new DiagramEdge("edge_review_finish", "review", "finish", "通过", DiagramLineType.Straight, DiagramArrow.Classic)
                ]);

            var preview = DiagramPreviewFactory.FromDocument(document);

            var labels = CollectText(preview).ToList();
            Assert.Contains("论文草稿", labels);
            Assert.Contains("导师审核", labels);
            Assert.Contains("定稿归档", labels);
            Assert.Contains("提交", labels);
            Assert.Contains("通过", labels);
        });
    }

    private static void RunOnSta(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception is not null)
        {
            throw exception;
        }
    }

    private static IEnumerable<string> CollectText(object element)
    {
        switch (element)
        {
            case TextBlock textBlock:
                yield return textBlock.Text;
                break;
            case ContentControl contentControl when contentControl.Content is not null:
                foreach (var label in CollectText(contentControl.Content))
                {
                    yield return label;
                }

                break;
            case Decorator decorator when decorator.Child is not null:
                foreach (var label in CollectText(decorator.Child))
                {
                    yield return label;
                }

                break;
            case Panel panel:
                foreach (var child in panel.Children)
                {
                    foreach (var label in CollectText(child))
                    {
                        yield return label;
                    }
                }

                break;
        }
    }
}
