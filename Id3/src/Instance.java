import java.util.Arrays;

public class Instance {
	
	public String[] attrList;
	public String label;
	public String prediction;
	
	
	public Instance(String[] attributes){
		attrList = attributes;
		label = attrList[0];
	}
	
	public Instance(Instance i){
		String[] list = new String[i.attrList.length];
		for(int j =0;j<i.attrList.length; j++){
			list[j]=i.attrList[j];
		}
		this.attrList= list;
		this.label = i.label;
		this.prediction = i.prediction;
	}

	public static void main(String[] args) {
		// TODO Auto-generated method stub

	}

	@Override
	public String toString() {
		return "Instance [attrList=" + Arrays.toString(attrList) + "]";
	}

}
