import model.DataObject;
import model.Student;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

class Cleaner {
//static methods

    public static List<Student> clean(List<DataObject> entries) {
        List<Student> s = new ArrayList<>();
        for (DataObject e : entries) {
            Student.Gender gender = cleanGender(e.gender);
            double age = cleanAge(e.age);
            double shoe = cleanShoe(e.shoesize);
            Student.Degree degree = cleanDegree(e.degree);
            String[] prg = cleanProgramming(e.programmingLanguage);
            double height = cleanHeight(e.height);
            Student.PhoneOS phoneOS = cleanPhoneOS(e.phoneOs);
            String[] commute = cleanCommute(e.commute);
            String[] playedGames = cleanGames(e.playedGames);
            Student.WhyCourse whyCourse = cleanWhyCourse(e.whyCourse);

            Student student = new Student(age, gender, shoe, height, commute, degree, prg, phoneOS, playedGames, whyCourse);
            System.out.println(student.toString());
            s.add(student);
        }
        return s;
    }

    private static Student.WhyCourse cleanWhyCourse(String whyCourse) {
        whyCourse = whyCourse.trim().toLowerCase();
        switch (whyCourse) {
            case "i am interested in the subject":
                return Student.WhyCourse.InterestedInSubject;
            case "this was a mandatory course for me":
                return Student.WhyCourse.Mandatory;
            case "it may help me to find a job":
                return Student.WhyCourse.HelpJob;
            case "the other optional courses were least appealing":
                return Student.WhyCourse.MostAppealing;
            default:
                return null;
        }


    }

    private static String[] cleanGames(String entry) {
        if (entry == null || entry.contains("I have not played any of these games")) return new String[0];
        String[] items = entry.toLowerCase().split(";");
        for (int i = 0; i < items.length; i++) {
            items[i] = items[i].trim().toLowerCase();
        }
        Arrays.sort(items, 0, items.length); //sort games
        return items;


    }


    private static String[] cleanCommute(String entry) {
        return entry.split(";");
    }

    private static Student.PhoneOS cleanPhoneOS(String entry) {
        switch (entry.trim().toLowerCase()) {
            case "android":
                return Student.PhoneOS.Android;
            case "ios":
                return Student.PhoneOS.iOS;
            case "windows":
                return Student.PhoneOS.Windows;
            case "ubuntu touch":
                return Student.PhoneOS.UbuntuTouch;
            default:
                return null;

        }
    }

    private static Student.Gender cleanGender(String entry) {

        switch (entry.trim().toLowerCase()) {
            case "male":
            case "m":
            case "man":
                return Student.Gender.Male;
            case "female":
            case "women":
            case "woman":
            case "f":
            case "w":
                return Student.Gender.Female;
            default:
                return null;
        }
    }

    private static double cleanHeight(String entry) {
        double h = parseNonNegativeInt(entry);
        if (h == 0) {
            h = parseNonNegativeDouble(entry);
        }

        return h > 145.0 && h < 240.0 ? h : 0; //greater than Dwarf and smaller than world tallest man
    }

    private static double cleanAge(String entry) {
        double age = parseNonNegativeInt(entry);
        return age > 15 && age < 99 ? age : 0;
    }


    private static double cleanShoe(String entry) {
        double shoe = parseNonNegativeDouble(entry);
        return shoe > 30 && shoe < 75 ? shoe : 0.0; //bigger than baby feet but smaller than biggest shoe size
    }

    private static Student.Degree cleanDegree(String entry) {
        switch (entry.trim().toLowerCase()) {
            case "sdt-se":
                return Student.Degree.SDTSE;
            case "sdt-dt":
                return Student.Degree.SDTDT;
            case "games-t":
                return Student.Degree.GAMES;
            default:
                return null;
        }
    }

    private static String[] cleanProgramming(String programmingLanguage) {
        programmingLanguage = programmingLanguage.replace('/', ',').replace(' ', ',').replaceAll(",,", ",");
        String[] p = programmingLanguage.split(",");

        for (int i = 0; i < p.length; i++) {
            p[i] = p[i].toLowerCase().trim();
        }
        return p;
    }

    private static int parseNonNegativeInt(String entry) {
        entry = entry.replaceAll("[^0-9]", "");
        try {
            int i = Integer.parseInt(entry);
            if (i < 0) {
                return 0;
            }
            return i;
        } catch (NumberFormatException ignored) {
            return 0;
        }
    }

    private static double parseNonNegativeDouble(String entry) {
        entry = entry.replaceAll("[^0-9,.]", "");
        try {
            double i;
            if (entry.contains(",")) {
                entry = entry.replace(',', '.');
            }
            if (entry.contains("-")) {
                String[] interval = entry.split("-");
                int n = interval.length;
                double lower = Double.parseDouble(interval[0]);
                double upper = Double.parseDouble(interval[n - 1]);
                i = (upper + lower) / n;
            } else {
                i = Double.parseDouble(entry);
            }
            if (i < 0) {
                return 0.0;
            }
            return i;
        } catch (NumberFormatException ignored) {
            return 0.0;
        }
    }


}

