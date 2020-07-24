package model;

import methods.Supervised.IEntity;

import java.util.ArrayList;
import java.util.List;

public class Student implements IEntity<Student> {
    public final double age;
    public final Gender gender;
    public final double shoesize;
    public final double height;
    public final String[] commute;
    public final Degree degree;
    public final String[] programmingLanguage;
    public final PhoneOS phoneOS;
    public final String[] playedGames;
    public final WhyCourse whyCourse;

    public Student(double age, Gender gender, double shoesize, double height, String[] commute,
                   Degree degree, String[] programmingLanguage, PhoneOS phoneOS, String[] playedGames, WhyCourse whyCourse) {
        this.age = age;
        this.gender = gender;
        this.shoesize = shoesize;
        this.height = height;
        this.commute = commute;
        this.degree = degree;
        this.programmingLanguage = programmingLanguage;
        this.phoneOS = phoneOS;
        this.playedGames = playedGames;
        this.whyCourse = whyCourse;
    }

    @Override
    public String toString() {
        return String.format("model.Student: Age: %s | Height: %s | Shoe: %s | Gender: %s | PhoneOS: %s " +
                        "| Degree: %s  WhyCourse %s | ",
                age, height, shoesize,gender, phoneOS, degree,
                getStrings(commute),
                getStrings(programmingLanguage),
                getStrings(playedGames),
                whyCourse);
    }

    private String getStrings(String[] s) {
        StringBuilder res = new StringBuilder();
        for (String p : s) {
            res.append(p).append(", ");
        }
        return res.substring(0, res.length());
    }

    /***
     * Returns the value of an Attribute based on its .class type object.
     * @param Attribute .class type object of its type
     * @return
     */
    public Object getAttributeValue(Object Attribute)
    {
        if(Attribute.equals(Degree.class))
        {
            return this.degree;
        }
        if(Attribute.equals(PhoneOS.class))
        {
            return this.phoneOS;
        }
        if(Attribute.equals(Gender.class))
        {
            return this.gender;
        }
        if(Attribute.equals(WhyCourse.class))
        {
            return this.whyCourse;
        }

        return null; //If we get down here something is wrong;
    }

    public List<Object> getAttributeList(Object toCompare)
    {
        ArrayList<Object> attributes = new ArrayList<>();
        if(toCompare.getClass()==WhyCourse.class) attributes.add(WhyCourse.class);
        if(toCompare.getClass()==Degree.class) attributes.add(Degree.class);
        if(toCompare.getClass()==Gender.class) attributes.add(Gender.class);
        if(toCompare.getClass()==PhoneOS.class) attributes.add(PhoneOS.class);

        return attributes;
    }


    @Override
    public int compareTo(Student other, Object toCompare) {
        int dist = 0;

        for (Object attr: this.getAttributeList(toCompare)) { //Don't include to compare

            if (this.getAttributeValue(attr)==null){
                if(other.getAttributeValue(attr) != null){
                    dist++;
                }
            }
            else if(!this.getAttributeValue(attr).equals(other.getAttributeValue(attr))) {
                dist++;
            }
        }

        return dist;
    }

    public enum Degree {
        SDTSE, SDTDT, GAMES
    }

    public enum Gender {
        Male, Female
    }

    public enum PhoneOS {
        Windows, iOS, Android, UbuntuTouch
    }

    public enum WhyCourse {
        Mandatory,
        InterestedInSubject,
        MostAppealing,
        HelpJob,
    }

}
