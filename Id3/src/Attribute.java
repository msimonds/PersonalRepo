import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map.Entry;

public class Attribute {
	
	public String name;
	
	public HashMap<String, Integer> valueMap; //has name of values connected to the number of occurences
	public HashMap<String, ArrayList<Instance>> instanceMap;
	public HashMap<String, Double> partialCounter; //maps the val String to the percentage that the val occurs in the dataset
	public String threshold;
	public boolean isCont;
	public int index;
	
	public Attribute(String attName){
		name = attName;
		valueMap = new  HashMap<String, Integer>();
		instanceMap = new HashMap<String, ArrayList<Instance>>();
		isCont = false;
		partialCounter= new HashMap<String, Double>();
	}
	public Attribute(String attName, boolean continuous){
		Attribute x = new Attribute(attName);
		x.isCont=true;
	}
	
	public Attribute(Attribute a){
		name = a.name;
		isCont = a.isCont;
		index = a.index;
		threshold = a.threshold;
		
		valueMap = new HashMap<String, Integer>();
		for(Entry<String, Integer> ent : a.valueMap.entrySet()){
			valueMap.put(ent.getKey(), ent.getValue());
		}
		
		instanceMap = new HashMap<String, ArrayList<Instance>>();
		for(Entry<String, ArrayList<Instance>> x : a.instanceMap.entrySet()){
			ArrayList list = new ArrayList<Instance>();
			for(Instance i : x.getValue()){
				list.add(i);
			}
			instanceMap.put(x.getKey(), list);
		}
		partialCounter = new HashMap<String, Double>();
		for(Entry<String,Double> en : a.partialCounter.entrySet()){
			partialCounter.put(en.getKey(), en.getValue());
		}
		
			
		
	}
	
	@Override
	public String toString() {
		return "Attribute [name=" + name + "]";
	}

}
