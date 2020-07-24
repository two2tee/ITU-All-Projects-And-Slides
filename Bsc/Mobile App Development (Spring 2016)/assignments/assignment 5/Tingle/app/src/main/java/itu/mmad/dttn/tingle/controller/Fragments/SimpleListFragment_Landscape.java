package itu.mmad.dttn.tingle.controller.Fragments;

import android.content.Context;
import android.content.res.Configuration;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import java.util.List;

import itu.mmad.dttn.tingle.R;
import itu.mmad.dttn.tingle.controller.BaseActivity;
import itu.mmad.dttn.tingle.controller.DetailedThingActivity;
import itu.mmad.dttn.tingle.model.Thing;
import itu.mmad.dttn.tingle.model.database.ThingsDatabase;

/**
 * Fragment for the list in detailed thing when in landscape mode.
 */
public class SimpleListFragment_Landscape extends Fragment {


    //Database
    private ThingsDatabase repository;

    //GUI
    private View view;
    private RecyclerView itemList;
    private ThingAdapter mAdapter;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
            view = inflater.inflate(R.layout.fragment_list_simple_landscape, container, false);
            repository = ((BaseActivity) getActivity()).getDatabase();

            setItemList();
            updateList();
            return view;
        }
        return null;
    }

    private void updateList() {
        List<Thing> things = repository.getAll();
        if (mAdapter == null) {
            mAdapter = new ThingAdapter(things);
            itemList.setAdapter(mAdapter);
        } else {
            mAdapter.setThings(things);
            mAdapter.notifyDataSetChanged();
        }
    }

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);
        repository = ((BaseActivity) getActivity()).getDatabase();
    }

    private void makeToast(String string) {
        Context context = getActivity().getApplicationContext();
        Toast.makeText(context, string, Toast.LENGTH_SHORT).show();
    }

    private void setItemList() {
        itemList = (RecyclerView) view.findViewById(R.id.item_list);
        itemList.setLayoutManager(new LinearLayoutManager(getActivity()));
    }


    //ViewHolder for list

    private class ThingHolder extends RecyclerView.ViewHolder implements View.OnClickListener {
        private final TextView mThingLabel;
        private Thing mThing;

        public ThingHolder(View itemView) {
            super(itemView);
            mThingLabel = (TextView) itemView.findViewById(R.id.list_item_text);
            itemView.setClickable(true);
            itemView.setOnClickListener(this);


        }

        public void BindThing(Thing thing) {
            mThingLabel.setText(thing.toString());
            mThing = thing;
        }

        @Override
        public void onClick(View v) {
            ((DetailedThingActivity) getActivity()).updateSelectedItemView(mThing.getId());
        }
    }


    //Adapter for list
    private class ThingAdapter extends RecyclerView.Adapter<ThingHolder> {
        private List<Thing> mThings;

        public ThingAdapter(List<Thing> things) {
            mThings = things;
        }

        @Override
        public ThingHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            LayoutInflater layoutInflater = LayoutInflater.from(getActivity());
            View view = layoutInflater.inflate(R.layout.list_view_row_item_simple, parent, false);
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
