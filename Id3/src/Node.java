

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map.Entry;
	 
public class Node {
	public String edgename;
	public String attribute;
	public HashMap<String, Node> children = new HashMap<String, Node>();
	public Node parent;
	public boolean isLeaf;
	public String label;
	public int distribution;

	 
	 public Node(Node parent, String edgename, String value, boolean leaf  ) {
		 this.parent = parent;
		 this.edgename = edgename;
		 this.attribute = value;
		 this.isLeaf = leaf;
	 }
	 public Node(String value, boolean leaf){
		 this.attribute = value;
		 this.isLeaf = leaf;
		 if(isLeaf){
			 this.attribute = "label";
		 }
	 }	
	 
	 
	 public Node(Node n) {		
		this.edgename = n.edgename;
		this.attribute = n.attribute;		
		this.parent = n.parent;
		this.isLeaf = n.isLeaf;
		this.label = n.label;
		this.distribution = n.distribution;
		for(Entry<String, Node> ent : n.children.entrySet()){
			this.children.put(ent.getKey(), new Node(ent.getValue()));
		}
	}
	@Override
	public String toString() {
		return "Node [ attribute=" + attribute+"]";
	}
	public void addChild(String edgename, String attribute){
		 Node child = new Node(this, edgename, attribute, false);
		 children.put(edgename, child);
		 
		 
	 }
	 public void addLeaf(String edgename, String label){
		 	Node leaf = new Node(this, edgename, label, true);
		 	
	 }
	 
	 public String getEdge() {
		 return edgename;
	 }
	 

	 public HashMap<String, Node> getChildren() {
		 return children;
	 }
	 
	 public Node getParent() {
		 return parent;
	 }
	
	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		Node other = (Node) obj;
		if (attribute == null) {
			if (other.attribute != null)
				return false;
		} else if (!attribute.equals(other.attribute))
			return false;
		
		
		if (edgename == null) {
			if (other.edgename != null)
				return false;
		} else if (!edgename.equals(other.edgename))
			return false;
		if (isLeaf != other.isLeaf)
			return false;
		
		
		return true;
	}
	 
	
}
