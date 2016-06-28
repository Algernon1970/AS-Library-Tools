Imports System.DirectoryServices.AccountManagement
Imports System.Windows.Forms
Imports AshbyTools

''' <summary>
''' Treeview modified to display contents of ActiveDirectory.
''' </summary>
Public Class ADTreeView
    Inherits System.Windows.Forms.TreeView
    Public Sub loadAD(ByRef ctx As PrincipalContext)
        Dim uTree As AshbyTools.UserTree = getUserTree(ctx)
        Dim currentHead As TreeNode = New TreeNode(uTree.name)
        Me.Nodes.Add(currentHead)
        For Each locationNode As UserTree In uTree.children
            buildTreeView(currentHead, locationNode)
        Next
    End Sub

    Private Sub buildTreeView(ByVal cHead As TreeNode, ByVal lNode As UserTree)
        Dim ncHead As TreeNode = New TreeNode(lNode.name)
        cHead.Nodes.Add(ncHead)
        If lNode.children IsNot Nothing Then
            For Each locationNode As UserTree In lNode.children
                buildTreeView(ncHead, locationNode)
            Next
        End If
    End Sub

End Class
