Imports System.DirectoryServices.AccountManagement
Imports System.Windows.Forms
Imports AshbyTools

''' <summary>
''' Treeview modified to display contents of ActiveDirectory.
''' </summary>
Public Class ADTreeView
    Inherits System.Windows.Forms.TreeView

    Dim uTree As AshbyTools.ObjectTree
    Public Sub loadUserAD(ByRef ctx As PrincipalContext)
        uTree = getUserTree(ctx)
        Dim currentHead As TreeNode = New TreeNode(uTree.name)
        Me.Nodes.Add(currentHead)
        For Each locationNode As ObjectTree In uTree.children
            buildTreeView(currentHead, locationNode)
        Next
    End Sub

    Public Sub loadGroupAD(ByRef ctx As PrincipalContext)
        uTree = getGroupTree(ctx)
        Dim currentHead As TreeNode = New TreeNode(uTree.name)
        Me.Nodes.Add(currentHead)
        For Each locationNode As ObjectTree In uTree.children
            buildTreeView(currentHead, locationNode)
        Next
    End Sub

    Private Sub buildTreeView(ByVal cHead As TreeNode, ByVal lNode As ObjectTree)
        Dim ncHead As TreeNode = New TreeNode(lNode.name)
        cHead.Nodes.Add(ncHead)
        If lNode.children IsNot Nothing Then
            For Each locationNode As ObjectTree In lNode.children
                buildTreeView(ncHead, locationNode)
            Next
        End If
    End Sub

    Public Function getUsersAt(ByVal loc As String, ctx As PrincipalContext) As List(Of UserPrincipalex)
        Dim retlist As New List(Of UserPrincipalex)
        uTree = getUserTree(ctx)
        Dim objectnodes As List(Of NodeContainer) = ADTools.findMatchingNode(loc, uTree).objectList
        For Each node As NodeContainer In objectnodes
            If node.type = TypeOfContainer.userex Then
                retlist.Add(node.userex)
            End If
        Next
        Return retlist
    End Function


End Class
