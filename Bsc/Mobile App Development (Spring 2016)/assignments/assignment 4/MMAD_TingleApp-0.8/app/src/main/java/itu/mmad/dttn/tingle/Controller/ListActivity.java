package itu.mmad.dttn.tingle.Controller;

import android.support.v7.app.AppCompatActivity;

/**
 * This class represents an activity to display all items created.
 *  @deprecated
 *  This activity has been replaced with a fragment
 *  If you want to use this class, please refer to older versions of Tingle.
 *  eg Tingle V0.5
 *
 */
public class ListActivity extends AppCompatActivity {

//    //Database
//    private IRepository<Thing> repository;
//
//    //GUI
//    private Button back;
//    private Button delete;
//    private ListView itemList;
//
//    private int selectedItemPos;
//
//    @Override
//    protected void onCreate(Bundle savedInstanceState) {
//        super.onCreate(savedInstanceState);
//        setContentView(R.layout.activity_list);
//
//        repository = InMemoryRepository.getInMemoryRepository();
//
//        setButtons();
//        selectedItemPos = -1;
//        setItemList();
//    }
//
//
//    private void makeToast(String string) {
//        Toast.makeText(ListActivity.this, string, Toast.LENGTH_SHORT).show();
//    }
//
//    private void setButtons() {
//
//        back = (Button) findViewById(R.id.back_button);
//        back.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//                Intent i = new Intent(ListActivity.this, TingleActivity.class);
//                startActivity(i);
//            }
//        });
//
//
//        delete = (Button) findViewById(R.id.delete_Button);
//        delete.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//                if (selectedItemPos == -1) {
//                    makeToast(getString(R.string.no_item_selected));
//                } else {
//                    repository.delete(selectedItemPos);
//                    setItemList();
//                    selectedItemPos = -1;
//                }
//            }
//        });
//
//    }
//
//
//    private void setItemList() {
//        Thing[] things = new Thing[repository.getTotalSize() + 1];
//        Iterator<Thing> items = repository.getAll();
//        int count = 0;
//        while (items.hasNext()) {
//            things[count] = items.next();
//            count++;
//        }
//
//        final ArrayAdapter adapter = new ArrayAdapter(this, R.layout.list_view_row_item, R.id.list_item_text, things);
//        itemList = (ListView) findViewById(R.id.item_list);
//        itemList.setAdapter(adapter);
//
//        itemList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
//            @Override
//            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
//                selectedItemPos = position;
//                Thing selectedItem = (Thing) adapter.getItem(position);
//                makeToast(getString(R.string.item_selected) + " " + selectedItem.getWhat());
//            }
//        });
//    }

}