package itu.mmad.dttn.tingle.controller.Fragments;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.os.OperationCanceledException;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.SearchView;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

import itu.mmad.dttn.tingle.R;
import itu.mmad.dttn.tingle.TingleApplication;
import itu.mmad.dttn.tingle.controller.BaseActivity;
import itu.mmad.dttn.tingle.controller.DetailedThingActivity;
import itu.mmad.dttn.tingle.model.Interfaces.ISort;
import itu.mmad.dttn.tingle.model.Searching.SearchHandler;
import itu.mmad.dttn.tingle.model.Thing;
import itu.mmad.dttn.tingle.model.database.ThingsDatabase;

/**
 * Fragment for the list page
 */
public class ListFragment extends Fragment {

    private ListFragmentEventListener mCallBack;
    //Search sort handler
    private SearchHandler mSearchHandler;

    //Database
    private ThingsDatabase repository;
    //GUI
    private View view;
    private RecyclerView itemList;
    private ThingAdapter mAdapter;
    private List<Thing> selectedItems;


    /**
     * Retrieve search handler and enable menu
     *
     * @param savedInstanceState
     */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setHasOptionsMenu(true);
        mSearchHandler = TingleApplication.getSearchHandler();

    }

    /**
     * inflates all widgets and retrieves database
     *
     * @param inflater           inflater
     * @param container          viewgroup
     * @param savedInstanceState bundle
     * @return View
     */
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        view = inflater.inflate(R.layout.fragment_list, container, false);
        repository = ((BaseActivity) getActivity()).getDatabase();
        selectedItems = new ArrayList<>();

        setItemList();
        updateList();

        return view;
    }

    /**
     * Updates list when app is resumed
     */
    @Override
    public void onResume() {
        super.onResume();
        updateList();
    }


    private void updateList() {
        List<Thing> things = repository.getAll();
        mSearchHandler.sortDefault(things);
        if (mAdapter == null) {
            mAdapter = new ThingAdapter(things);
            itemList.setAdapter(mAdapter);
        } else {
            mAdapter.setThings(things);
            mAdapter.notifyDataSetChanged();
        }
    }

    /**
     * Check if event listeners has been implemented and retrieve database
     *
     * @param context context
     */
    @Override
    public void onAttach(Context context) {
        super.onAttach(context);

        //Checks if parent activity has implemented the
        //callback interface
        try {
            mCallBack = (ListFragmentEventListener) context;
            repository = ((BaseActivity) getActivity()).getDatabase();

        } catch (ClassCastException e) {
            throw new ClassCastException(context.toString() + " must implement ListFragmentEventListener");
        }
    }

    /**
     * Sets the menu
     *
     * @param menu
     * @param inflater
     */
    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {
        menu.clear();

        inflater.inflate(R.menu.fragment_list, menu);
        super.onCreateOptionsMenu(menu, inflater);

        //Set groups
        MenuItem defaultSearchItem = menu.findItem(R.id.search_what);
        defaultSearchItem.setChecked(true);

        //Set search view
        final MenuItem searchItem = menu.findItem(R.id.menu_item_search);
        final SearchView searchView = (SearchView) searchItem.getActionView();

        searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public boolean onQueryTextSubmit(String query) {
                List<Thing> result = mSearchHandler.search(query.toLowerCase().trim(), repository.getAll());
                if (result == null) {
                    makeToast(getString(R.string.item_NotFound_toast));
                    return false;
                }
                mAdapter.setThings(result);
                mAdapter.notifyDataSetChanged();
                return true;
            }

            @Override
            public boolean onQueryTextChange(String newText) {
                if (newText.length() == 0) {
                    updateList(); //reset list
                } else {
                    List<Thing> result = mSearchHandler.search(newText.toLowerCase().trim(), repository.getAll());
                    if (result == null) {
                        return false;
                    }
                    mAdapter.setThings(result);
                    mAdapter.notifyDataSetChanged();
                }
                return true;
            }
        });

    }

    /**
     * Perform action based on item selected in menu
     *
     * @param item selected item
     * @return boolean
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.delete_Button:
                deleteItem();
                return true;

            case R.id.back_button:
                goBack();
                return true;

            case R.id.search_what:
                item.setChecked(true);
                mSearchHandler.setCurrentSearchType(SearchHandler.searchType.SEARCH_WHAT);
                return true;

            case R.id.search_where:
                item.setChecked(true);
                mSearchHandler.setCurrentSearchType(SearchHandler.searchType.SEARCH_WHERE);
                return true;

            case R.id.sortWhat:
                setSortedList(ISort.type.SORT_WHAT);
                return true;

            case R.id.sortWhere:
                setSortedList(ISort.type.SORT_WHERE);
                return true;

            case R.id.sortDate:
                setSortedList(ISort.type.SORT_DATE);
                return true;

            default:
                return super.onOptionsItemSelected(item);
        }

    }

    /**
     * Sorts list and update view
     *
     * @param sortType type of sort
     */
    private void setSortedList(ISort.type sortType) {
        if (mAdapter.mThings.size() == 0) return;
        List<Thing> result = mSearchHandler.sort(mAdapter.mThings, sortType);
        mAdapter.setThings(result);
        mAdapter.notifyDataSetChanged();
    }


    private void makeToast(String string) {
        Context context = getActivity().getApplicationContext();
        Toast.makeText(context, string, Toast.LENGTH_SHORT).show();
    }

    private void goBack() {
        //Portrait mode show go to list button
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {
            mCallBack.goBack();
        }
    }

    private void deleteItem() {
        if (selectedItems.isEmpty()) {
            makeToast(getString(R.string.no_item_selected));
        } else {

            new AlertDialog.Builder(getActivity()).setIcon(android.R.drawable.ic_dialog_alert).setTitle(R.string.warning).setMessage(R.string.delete_question).setPositiveButton(R.string.yes, new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    try {
                        for (Thing thing : selectedItems) {
                            repository.delete(thing.getId());
                            mAdapter.mThings.remove(thing);
                        }
                        selectedItems.clear();
                        updateList();

                    } catch (OperationCanceledException e) {
                        makeToast(getString(R.string.something_Went_Wrong));
                    }

                }

            }).setNegativeButton(R.string.no, null).show();
        }
    }

    private void setItemList() {
        itemList = (RecyclerView) view.findViewById(R.id.item_list);
        itemList.setLayoutManager(new LinearLayoutManager(getActivity()));
    }

    /**
     * Event listeners for listFragment
     */
    public interface ListFragmentEventListener {
        void goBack();
    }


    //ViewHolder for list

    /**
     * ViewHolder that is responsible for setting up elements in recycleView
     */
    private class ThingHolder extends RecyclerView.ViewHolder implements View.OnClickListener {
        private final TextView mThingLabel;
        private final CheckBox mSelectedCheckBox;
        private Thing mThing;

        public ThingHolder(View itemView) {
            super(itemView);
            mThingLabel = (TextView) itemView.findViewById(R.id.list_item_text);
            mSelectedCheckBox = (CheckBox) itemView.findViewById(R.id.list_checkbox);
            setCheckboxes();
            itemView.setOnClickListener(this);


        }

        public void BindThing(Thing thing) {
            mThingLabel.setText(thing.toString());
            mSelectedCheckBox.setChecked(false);
            mThing = thing;
        }

        private void setCheckboxes() {
            mSelectedCheckBox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
                @Override
                public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                    if (isChecked) {
                        selectedItems.add(mThing);
                    } else {
                        selectedItems.remove(mThing);
                    }
                }
            });
        }

        @Override
        public void onClick(View v) {
            Intent intent = DetailedThingActivity.newIntent(getActivity(), mThing.getId());
            startActivity(intent);

        }


    }

    //Adapter for list

    /**
     * ViewAdapter that sets up the recycleView and thing holder based on a list of things
     */
    private class ThingAdapter extends RecyclerView.Adapter<ThingHolder> {
        private List<Thing> mThings;

        public ThingAdapter(List<Thing> things) {
            mThings = things;
        }

        @Override
        public ThingHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            LayoutInflater layoutInflater = LayoutInflater.from(getActivity());
            View view = layoutInflater.inflate(R.layout.list_view_row_item, parent, false);
            return new ThingHolder(view);
        }

        @Override
        public void onBindViewHolder(ThingHolder holder, int position) {
            Thing thing = mThings.get(position);
            holder.BindThing(thing);
        }

        @Override
        public int getItemCount() {
            return mThings.size();
        }

        public void setThings(List<Thing> things) {
            mThings = things;
        }
    }


}