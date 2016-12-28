import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map.Entry;
import java.util.Random;
import java.util.Scanner;

public class MainCl {	
	public static Attribute[] attList;
	public static Instance[] instances;
	public static int algo;
	public static HashMap<LinkedList<Node>, ArrayList<LinkedList<Node>>> ruleMap;
	public static int totalmatrix =0;
	public static int diag=0;
	
	
	public static void main(String[] args) {
		
		String alg = args[1];
		
		if(alg.equals("ID3")){
			algo=0;
		}else if(alg.equals("C4.5")){
			algo=1;
			ruleMap = new HashMap<LinkedList<Node>, ArrayList<LinkedList<Node>>>();
		}else if(alg.equals("C4.5NP")){
			algo=2;
		}
		else if(alg.equals("C4.5NSI")){
			algo=3;
		}
		else{
			System.out.println("Enter a real algorithm");
			return;
		}
		String path = args[0];
		int seed = Integer.parseInt(args[2]);
		
		ArrayList<Instance> S = new ArrayList<Instance>();
		ArrayList<Attribute> A = new ArrayList<Attribute>();
		
		File f = new File(path);
		Scanner sc=null;
		try {
			sc = new Scanner(f);
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		String line = sc.nextLine();
		Scanner linesc = new Scanner(line);
		int i = 0;
		while (linesc.hasNext()){
			//add each attribute to the array of attribute 			
			for(String str : linesc.nextLine().split(",")){
				Attribute newAtt = new Attribute(str);
				newAtt.index = i;
				A.add(newAtt);				
				i++;
			}			
		}
		i=0;
		while(sc.hasNextLine()){
			//grab the next line 
			line = sc.nextLine();
			//populate initial list of instances
			String[] instArr = line.split(",");
			Instance inst = new Instance(instArr);
			S.add(inst);
				
		}
		
		A = buildAttributes(A, S);	
		ArrayList<Attribute> trainingA;
		ArrayList<Instance> trainingS;
		ArrayList<Instance> testS;
		ArrayList<Instance> validationS = null;
		if(algo==1){
			
			A = c4(S, A);
			Collections.shuffle(S, new Random(seed));
			trainingS = new ArrayList<Instance>( S.subList(0, (int) (S.size()*0.6)));
			validationS = new ArrayList<Instance>( S.subList( (int) (S.size()*0.6), (int) (S.size()*0.7)));
			System.out.println(trainingS.size());
			testS = new ArrayList<Instance>( S.subList((int) (S.size()*0.6), S.size()));
			trainingA = copyList(A);			
		}else{
		
		Collections.shuffle(S, new Random(seed));
		trainingS = new ArrayList<Instance>( S.subList(0, (int) (S.size()*0.6)));
		System.out.println(trainingS.size());
		testS = new ArrayList<Instance>( S.subList((int) (S.size()*0.6), S.size()));
		trainingA = copyList(A);
		}
		
		Node root = new Node("Start", false);		
		root.children.put("n/a", id3(trainingA, trainingS));		
		if(algo==1 || algo==3){
			prune(root, validationS);
		}
		int[][] r = test(root.children.get("n/a"), testS, A);	
		
		try{
		File fout = new File("out.txt");
		FileOutputStream fos = new FileOutputStream(fout);	 
		BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(fos));
	 
		
			for(int ii = 0; ii < A.get(0).valueMap.size(); ii++)
			   {
			      for(int j = 0; j < A.get(0).valueMap.size(); j++)
			      {
			         bw.write(r[ii][j] + ",");
			      }
			      bw.newLine();
			   }
			bw.write("predicted_right="+r[0]+" predicted_wrong="+r[1]);
			bw.newLine();
			 
		bw.close();
		} catch(Exception E){
			System.out.print("ha" +E.getStackTrace());
		}
		
		
		
	}
	
	
	//Finds a good threshold for continuous values and attaches the related subsets to the attribute
	public static Attribute threshold(ArrayList<Instance> S,ArrayList<Attribute> A, Attribute a){				
		//find the threshold values		
			//sort the list of instances S
		ArrayList<Instance> Sn = copyListi(S);
		
		Collections.sort(Sn, new Comparator<Object>() {

	        public int compare(Object o1, Object o2) {
	        	String f = ((Instance) o1).attrList[a.index];
	        	String g = ((Instance) o2).attrList[a.index];
	        	if(f.equals("?")){
	        		f="10000";
	        	}
	        	if(g.equals("?")){
	        		g="100000";
	        	}
	        	Double  x1 = Double.parseDouble(f);	            
	        	Double  x2 = Double.parseDouble(g);
	            
	            return x1.compareTo(x2);
	            
	    }});
		//PROBABLY NEED AN IF STATEMENT TO CATCH IF THERES ONLY ONE ITEM IN ARRAY
		String prev=Sn.get(0).attrList[a.index];
		int count=0;
		
		ArrayList<Threshold> possibleThresh = new ArrayList<Threshold>();
			//finds possible threshold ArrayList<int>
		for(Instance ins : Sn){
			if(!prev.equals("?") && !ins.attrList[a.index].equals("?")){				
				if(!ins.attrList[a.index].equals(prev)){					Double j = Double.parseDouble(prev);
					Double k = Double.parseDouble(ins.attrList[a.index]);
					Double div = j/k;
					possibleThresh.add(new Threshold(div.toString(), count-1, count));
				}
				prev = ins.attrList[a.index];					
			}
			count++;
		}
			//take a thresh, build Sv for it, calculate gain and store the gain in a 
		for(Threshold td : possibleThresh){
			buildSvThresh(Sn, a, td);
			//td.gain = gain(A.get(0), Sn);
		}
		int maxIndex = findMaxThresh(possibleThresh);
		//calculate a Sv, stored in instanceMap for the attribute
		Attribute attr =  buildSvThresh(Sn, a, possibleThresh.get(maxIndex) );
		A = partialCounts(A);
		return attr;		
	}
	
	//returns a new attribute with Sv attached to the instance/value map
	public static Attribute buildSvThresh(ArrayList<Instance> S, Attribute A, Threshold t){
		//this splits the instances into two and returns a copy of an attribute where Sv is loaded
		Attribute a = new Attribute(A);
		ArrayList<Instance> z = copyListi(S);
		List<Instance> temp = (List<Instance>) z.subList(0,  t.start);	
		ArrayList<Instance> arl = listToArr(temp);
		a.instanceMap.put("t1", arl);
		a.valueMap.put("t1", arl.size());
		
		temp = (List<Instance>) z.subList(t.start+1, z.size());
		arl = listToArr(temp);
		a.instanceMap.put("t2", arl);
		a.valueMap.put("t2", arl.size());
		return a;
	}
	
	public static ArrayList<Instance> listToArr(List<Instance> l){
		ArrayList<Instance> narr = new ArrayList<Instance>();
		for(Instance ins : l){
			narr.add(ins);
		}
		return narr;
		
	}
	
	
	public static int[][] test(Node start, ArrayList<Instance> S, ArrayList<Attribute> A){
		//HashMap<String, ArrayList<Integer>> matrix = new HashMap<String, ArrayList<Integer>();
		//find where the label is in A, use that index as the row, then calculate the matrix
		int[][] matrix = new int[A.get(0).valueMap.size()][A.get(0).valueMap.size()];
		int right = 0;
		int wrong=0;
		int[] re = new int[2];
		
		for(Instance s : S){
			predict(start, s, A);
			if(s.label.equals(s.prediction)){
				right++;
			}else{
				wrong++;
			}
			int row=0;
			int col=0;
			boolean x=false;
			for(Entry<String, Integer> v1 : A.get(0).valueMap.entrySet()){
				row=0;
				if(!x){
					for(Entry<String, Integer> v2 : A.get(0).valueMap.entrySet()){
						String a1 = v1.getKey();
						String a2 = v2.getKey();
						if((s.label.equals(a1)) && (s.prediction.equals(a2))){
							matrix[row][col] += 1;
							totalmatrix++;
							x=true;
						}else if((s.label.equals(a2)) && (s.prediction.equals(a1))){
							matrix[col][row] += 1;
							totalmatrix++;
							x=true;
						}else if((s.label.equals(a1)) && (s.prediction.equals(a1))){
							matrix[row][row] += 1;
							diag++;
							totalmatrix++;
							x=true;
						}else if((s.label.equals(a2)) && (s.prediction.equals(a2))){
							matrix[col][col] +=1;
							totalmatrix++;
							diag++;
							x=true;
						}					
						row++;
				}
				}
				col++;
			}
					
		}
		for(int i = 0; i < A.get(0).valueMap.size(); i++)
		   {
		      for(int j = 0; j < A.get(0).valueMap.size(); j++)
		      {
		         System.out.print(matrix[i][j] + ",");
		      }
		      System.out.println();
		   }
		
		return matrix;
	}
	
	public static void predict(Node N, Instance S, ArrayList<Attribute> A){
		//recurse through tree  until you get a prediction
		//loop through attribute values if one matches the current node then find the corresponding value 
		//on the instance and and follow that child branch
		
		if(N.attribute.equals("label")){
			S.prediction =N.label;
			//System.out.println(Arrays.toString(S.attrList));
			//System.out.println("predict="+S.prediction +" label="+ S.label);
			return;
		}
		for(int i=0;i<A.size();i++){
			if(A.get(i).name.equals(N.attribute)){
				predict(N.children.get(S.attrList[i]), S, A);
			}
		}		
	}
	

	//Returns index of the max of the array	
	public static int findMax(double[] arr){
		int max=0;
		for(int i=0; i<arr.length;i++){
			if(arr[max]<arr[i]){
				max = i;
			}
		}
		return max;
	}
	
	//Returns index of the max of the array	
		public static int findMaxThresh(ArrayList<Threshold> t){
			int max=0;
			for(int i=0; i<t.size();i++){
				if(t.get(max).gain<t.get(i).gain){
					max = i;
				}
			}
			return max;
		}
	
	//Returns the best a* attribute from a given list of attributes	(not including A[0] which is the label itself
	public static Attribute bestA(ArrayList<Attribute> A, ArrayList<Instance> S){		
		
		double[] gainList = new double[A.size()];
		gainList[0] = -1000;
		for(int i=1; i<A.size(); i++){
			if(A.get(i)!=null){
				 double t = gain(A.get(i), A, S);
				 if(algo==(1) || algo==2){
					double si =splitInfo(A.get(i));
					 if(si<0.1){
						 si=0.1;
					 }
					 t=t/si;
				 }
				 gainList[i]= t;
				
			}else{
				gainList[i]=-1000;
			}
		}				return A.get( findMax(gainList) );		
	}
	
	public static double splitInfo(Attribute a){
		double[] sums = new double[2200]; //sums[0] = total number of instances; sums[1]=number of val1..
		int size=0;
		int index=1;
		int temp = 0;
		for(Entry<String, Integer> entry : a.valueMap.entrySet()){
			String key = entry.getKey();
			 temp+=a.valueMap.get(key);
			 sums[index] = a.valueMap.get(key);
			 index++;
			 size++;
		}
		sums[0]=temp;
		double total=0;		
		
		for(int i=1;i<size+1;i++){			
			if(sums[i]!=0){				
				double c = sums[i]/sums[0];			
				total = total + (c * (Math.log(c)/Math.log(2)));
			}
		}
		
				
		
		return (total * -1);
	}

	//Calculate the Gain(A, S) for an attribute
	public static double gain(Attribute a, ArrayList<Attribute> a2, ArrayList<Instance> S){
		
			double entropyS = entropy( a2.get(0), S);
			double sum =0;
			for (Entry<String, Integer> val : a.valueMap.entrySet()){
				double sizeV = val.getValue();
				double sizeS = S.size();
				if(sizeV>0){
					sum+= (sizeV/sizeS)*entropy(a2.get(0), a.instanceMap.get(val.getKey()));
				}
			}		
			return (entropyS-sum);		
	}
	
	//Calculates entropy of a given attribute	
	public static double entropy( Attribute label, ArrayList<Instance> S){
		double[] sums = new double[2200]; //sums[0] = total number of instances; sums[1]=number of val1..		
		int temp = 0;
		
			for(Instance ins : S){
				int count=1;				
				
				for(Entry<String, Integer> labE : label.valueMap.entrySet()){
					if(labE.getKey().equals(ins.label)){
						sums[count] += 1;
						temp++;
						
						break;
					}
					count++;
					
				}					 
			}	
		
		sums[0]=temp;
		double total=0;		
		
		for(int i=1;i<label.valueMap.size()+1;i++){
			//!!!Is this correct???
			if(sums[i]!=0){				
				double c = sums[i]/sums[0];			
				total = total + (c * (Math.log(c)/Math.log(2)));
			}
		}
		total = total * (-1);
		return (total);		
	}	
	
	//takes in the current tree, a set of attributes, and a set of instances
	public static Node id3(ArrayList<Attribute> A, ArrayList<Instance> S ){		
		Node N=null;
		A = buildSv(A, S); //calculates the values of each possible Sv for each attribute
		if(A.size()==1){
			
			String label = mostCommonLabel(A.get(0), S);
			N=new Node("label", true);
			N.label=label;
			
			System.out.println(label+"* ");
			
		}else if(equalLabels(S, A.get(0))){
			String l = S.get(0).attrList[0];
			System.out.println(l+"** ");
			N = new Node("label", true);
			N.label=l;			
		}else{			
			Attribute a = bestA(A, S);
			
			if((algo>0) && a.isCont){				
				threshold(S, A, a);
				
				
			}
				String label = mostCommonLabel(A.get(0), S);
				N = new Node(a.name, false);
				for(Entry<String, Integer> entry : a.valueMap.entrySet()){
					String val = entry.getKey();
					if(a.valueMap.get(val)==0){
						N = new Node("label", true);
						N.label= label;						
						System.out.println( label +"*** ");
					} else{					
						ArrayList<Attribute> newA = cleanAttributes(A);
						newA = removeA(newA, a);
						System.out.print(a.name +"--"+val + "-->");
						N.children.put(val, id3(newA, a.instanceMap.get(val)));
						N.children.get(val).parent = N;
					}
				}			
		}		
		return N;		
	}
	
	public static ArrayList<Attribute> removeA(ArrayList<Attribute> A, Attribute a){
		for(int i =0;i<A.size();i++){
			if(A.get(i) !=null){
				if (A.get(i).name.equals(a.name)){
					A.remove(i);
					A.add(i, null);
				}
			}
		}
		return A;
	}
	
	public static ArrayList<Attribute> copyList(ArrayList<Attribute> A){
		ArrayList<Attribute> temp = new ArrayList<Attribute>();
		for(Attribute attr : A){
			if(attr==null){
				temp.add(null);
			}else{
				Attribute newAtt = new Attribute(attr);
				temp.add(newAtt);
			}
		}
		return temp;
	}
	
	//returns a copy of copies a list of instances
	public static ArrayList<Instance> copyListi(ArrayList<Instance> A){
		ArrayList<Instance> temp = new ArrayList<Instance>();
		for(Instance attr : A){
			if(attr==null){
				temp.add(null);
			}else{
				Instance newAtt = new Instance(attr);
				temp.add(newAtt);
			}
		}
		return temp;
	}
	
	//returns a new cleaned ArrayList
	public static ArrayList<Attribute> cleanAttributes(ArrayList<Attribute> A){
		ArrayList<Attribute> temp = copyList(A);
		
		
		for(Attribute att : temp){
			if(att!=null){
				for(Entry<String, Integer> entry : att.valueMap.entrySet()){
					String key = entry.getKey();
					att.valueMap.put(key, 0);
					att.instanceMap.put(key, new ArrayList<Instance>());
				}
			}
		}
		
		return temp;
	}
	
	public static String mostCommonLabel(Attribute A, ArrayList<Instance> S){
		Attribute helper = new Attribute("label");
		
		for(Instance ins : S){
			String at = ins.attrList[0];
			if(helper.valueMap.get(at)==null){
				helper.valueMap.put(at, 1);
			} else{
				helper.valueMap.put(at, helper.valueMap.get(at) + 1);
			}
		}
		
		String max=null;
		int i=0;
		for(Entry<String, Integer> entry : helper.valueMap.entrySet()){
			String val = entry.getKey();
			if(i==0){
				max = val;
				i++;
			}
			if(helper.valueMap.get(max) < helper.valueMap.get(val)){
				max = val;
			}
		}
		return max;
	}
	
	public static ArrayList<Attribute> buildSv(ArrayList<Attribute> a, ArrayList<Instance> s){
		//at each instance go through each column(attribute) and see what value it matches up with
		//update the count of the valuemap in the corresponding attribute
		for(Instance inst : s){
			int i = 0;
			for(String attrVal : inst.attrList){
				//we only care about attributes that are in our list
				if(a.get(i)!=null){
					for(Entry<String, Integer> entry : a.get(i).valueMap.entrySet() ){
						String value = entry.getKey();
						if(attrVal.equals(value)){
							if(a.get(i).valueMap.get(attrVal)==null){
								a.get(i).valueMap.put(attrVal, 1);
							}else{
								a.get(i).valueMap.put(attrVal, a.get(i).valueMap.get(attrVal)+1 );
							}
							if(a.get(i).instanceMap.get(attrVal)==null){
								ArrayList<Instance> list = new ArrayList<Instance>();
								list.add(inst);
								a.get(i).instanceMap.put(attrVal, list);
							} else {
								ArrayList<Instance> l = a.get(i).instanceMap.get(attrVal);
								l.add(inst);
								a.get(i).instanceMap.put(attrVal, l);
							}	
						}
					}					
				}
				i++;
			}		
		}	
		if(algo==1){
			a = partialCounts(a);
		}
		return a;		
	}
	//686 right id3 optical digit 
	//147 right c4.5 optical digit (w/ split info)
	//
	
	public static boolean equalLabels(ArrayList<Instance> S, Attribute a){
		int x = 0;
		int count=0;
		if(S.size()>1){
			for(Entry<String, Integer> ent : a.valueMap.entrySet()){
				if(ent.getValue()>0){
					count++;
				}
			}
			if(count>1){
				return false;
			}
		}
		return true;
		
		/*
		while(x<S.size()){
			if(S.get(x).attrList[0] == S.get(x+1).attrList[0]){
				x++;
			} else{
				return false;
			}			
		}
		return true;*/
	}
	
	//Adds keys (diff possible values of the attribute) to valuemap hashmap in A
	public static ArrayList<Attribute> buildAttributes(ArrayList<Attribute> A, ArrayList<Instance> S){
		for(Instance inst : S){
			//each instance loop through each attribute look at the string value
				//if not added then go to next attribute
			int i = 0;
			for(String attrVal : inst.attrList){
				if(A.get(i).valueMap.get(attrVal)==null){
					A.get(i).valueMap.put(attrVal, 0);
				}
				i++;
			}
			
		}
		//It's not doing things correctly!!!
		return A;
	}
	
	public static ArrayList<Attribute> c4(ArrayList<Instance> S, ArrayList<Attribute> A){
		
		
		//after building the list of attributes check the valuemap sizes for each attribute, if >20 then isCont=true
		for (Attribute a : A){
			if(a.valueMap.size()>20){
				a.isCont=true;				
			}
			
			
		}		
		return A;		
	}
	
	
	public static ArrayList<Attribute> partialCounts(ArrayList<Attribute> A){	
		//update partial counter for each by counting the # of occurences in each(sum of values in value map - "?" vals)
				//	and dividing the number by the value found in the valueMap	
				//  next remove each occurence of "?" from instance and valueMap
		for(Attribute a : A){
			if(a!=null){
				double count = 0;
				for(Entry<String, Integer> ent : a.valueMap.entrySet()){
					if(!ent.getKey().equals("?")){
						count = count +ent.getValue();
					}
				}
				for(Entry<String, Integer> entt : a.valueMap.entrySet()){
					if(!entt.getKey().equals("?")){
						//Set the partial count
						Double total = entt.getValue()/count; 
						a.partialCounter.put(entt.getKey(), total);
					} else{
						if(a.instanceMap.get(entt.getKey())!= null){
						//add the values of the "?"value instance map to each other values's instance map 
							for(Instance ins : a.instanceMap.get(entt.getKey())){
								
								//add it to the arraylist of instances if it is a  
									for(Entry<String, ArrayList<Instance>> i : a.instanceMap.entrySet()){
										if(!i.getKey().equals("?")){
											i.getValue().add(ins);
											Integer tmp = a.valueMap.get(i.getKey());
											a.valueMap.put(i.getKey(), tmp+1);
										}
									}
								
								}
						}
					}
					
				}
			}
			
		}
		
		return A;
	}
	
	//Prune the tree 		
	public static void prune(Node root, ArrayList<Instance> insList ){
		
		toRules(root.children.get("n/a"), new LinkedList<Node>());
		//populate the alternatives to each rull in the ruleMap		
		for(Entry<LinkedList<Node>, ArrayList<LinkedList<Node>>> ent : ruleMap.entrySet()){
			ent.getKey();
		}		 
		boolean optimal=false;
		while(!optimal){
			//make list of alternatives to the rule map (needs to be done until best rule has been picked)
			addAlternatives();
			
			//test the alternatives and create a new map with new keys==values set ruleMap= to new map 
			//return true if all the new map is the same as the old map
			optimal = testAlternatives(insList);
			
		}
		
	}
	
	//updates ruleMap to be populated with rules 
	public static void toRules(Node N, LinkedList<Node> list){
		if(N.attribute.equals("label")){			
			list.add(N);
			ArrayList<LinkedList<Node>> n = new ArrayList<LinkedList<Node>>();
			n.add(list);
			ruleMap.put(list, n);
			return;
			//ruleMap.put(key, key);
					
		}else{
			//add to linked list at position 0
			//for each child
			list.add(N);
			
			for(Entry<String, Node> ent : N.children.entrySet())							
				toRules(ent.getValue(), list);			
			
		}
	}
	public static boolean testAlternatives(ArrayList<Instance> insList){
		
		boolean opt = true;
		HashMap<LinkedList<Node>, ArrayList<LinkedList<Node>>> newMap = new HashMap<LinkedList<Node>, ArrayList<LinkedList<Node>>>();
		for(Entry<LinkedList<Node>, ArrayList<LinkedList<Node>>> ent : ruleMap.entrySet()){
			//check if the bestRule is the same as the new rule
			LinkedList<Node> best = bestRule(ent.getValue(), insList);
			if(!ruleIsEqual(ent.getKey(), best)){
				opt=false;
			}
			ArrayList<LinkedList<Node>> newAr = new ArrayList<LinkedList<Node>>();
			newAr.add(best);
			newMap.put(best, newAr);
		}
		ruleMap = newMap;
		return opt;
	}
	
	public static boolean ruleIsEqual(LinkedList<Node> x, LinkedList<Node> y){
		if(x.size()!=y.size()){
			return false;
		}
		for(int i=0; i<x.size(); i++){
			if(!(x.get(i).attribute.equals(y.get(i).attribute)) &&
					!(x.get(i).edgename.equals(y.get(i).edgename))){
				return false;				
			}
		}
		return true;
	}
	
	public static LinkedList<Node> bestRule(ArrayList<LinkedList<Node>> list, ArrayList<Instance> S){
		//make a list to add number predicted correctly
		//for each instance
			//foreach linkedList of nodes: 
				//foreach node:
					//if the node is a label check if the label matches:
						//update the accuracy list
					//check if the node edge and attribute matches the corresponding attribute in the instance S
		double[] accurate = new double[list.size()];
		for(Instance s : S){
			for(int i=0; i<list.size(); i++){				
				for(int j=1;j<list.get(i).size();j++){
					Node n = list.get(i).get(j);
					Node prev = list.get(i).get(j-1);
					if(n.attribute.equals("label")){
						if(n.label.equals(s.attrList[0])){
							accurate[i] +=1;
						}
					} else {
						//this boolean ensures we only look at pertinent instances
						boolean go= false;
						//cycle through the attrList
						//check if the attribute of the prev node and the edgename of the current node
						for(int ind=1; ind<attList.length;ind++){
							if(attList[i].name.equals(prev.attribute) && s.attrList[i].equals(n.edgename)){
								if((ind+1==attList.length)){
									go=true;
								}
							}
						}
						if(!go){
							break;
						}
						
						
					}
				}
			}
			
		}
		return copyLinked(list.get(findMax(accurate)));
		
		
	}
	
	// adds the alternatives to the hashmap by removing each node once, updates ruleMap
	public static void addAlternatives()
	{
		for(Entry<LinkedList<Node>, ArrayList<LinkedList<Node>>> ent: ruleMap.entrySet()){
			LinkedList<Node> original = ent.getKey();
			LinkedList<Node> copy = copyLinked(original);
			for(int i=0; i<original.size(); i++){
				Node rm = copy.remove(i);
				ruleMap.get(original).add(i+1, copy);
				copy.add(i, rm);
			}			
		}		
		return;
	}
	
	public static <T> LinkedList<T> copyLinked(LinkedList<T> list){
		LinkedList<T> newList = new LinkedList<T>();
		for(T object : list){
			newList.add(object);
		}
		return newList;	
			
	}
}



class Threshold{
	public int start;//the index of the instance in instance list to the left 
	public int end;
	public String value;
	public double gain;
	
	public Threshold(String v, int x, int y){
		start = x;
		end = y;
		this.value = v;
	}
	
	
	
}
