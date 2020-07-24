package model;

public class DataObject {
    public final String age;
    public final String gender;
    public final String shoesize;
    public final String degree;
    public final String height;
    public final String programmingLanguage;
    public final String commute;
    public final String phoneOs;
    public final String playedGames;
    public final String whyCourse;


    public DataObject(String age, String gender, String shoesize, String degree, String height, String programmingLanguage, String commute, String phoneOs, String playedGames, String whyCourse) {
        this.age = age;
        this.gender = gender;
        this.shoesize = shoesize;
        this.degree = degree;
        this.height = height;
        this.programmingLanguage = programmingLanguage;
        this.commute = commute;
        this.phoneOs = phoneOs;
        this.playedGames = playedGames;
        this.whyCourse = whyCourse;
    }

    @Override
    public String toString() {
        return String.format("Entry: Age: %s | Gender: %s | Shoe: %s | Degree: %s | Programming: %s | Height: %s | Commute: %s | phoneOS %s | Why course: %s   | played games: %s ", age, gender, shoesize, degree, programmingLanguage, height, commute, phoneOs, whyCourse, playedGames);
    }
}

